namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using System.Threading.Tasks;
	using ImpossibleOdds.Serialization.Caching;

	/// <summary>
	/// A (de)serialization processor to process custom object to dictionary-like data structures.
	/// </summary>
	public class CustomObjectLookupProcessor : AbstractCustomObjectProcessor, IDeserializationToTargetProcessor
	{
		private readonly bool requiresMarking = false;
		private readonly ILookupSerializationDefinition definition = null;
		private readonly ILookupTypeResolveSupport typeResolveSupport = null;
		private readonly IRequiredValueSupport requiredValueSupport = null;
		private readonly IParallelProcessingSupport parallelProcessingSupport = null;

		/// <summary>
		/// Does the serialization definition have support for type resolve parameters?
		/// </summary>
		public bool SupportsTypeResolvement
		{
			get => typeResolveSupport != null;
		}

		/// <summary>
		/// Does the serialization definition have support for required values?
		/// </summary>
		public bool SupportsRequiredValues
		{
			get => requiredValueSupport != null;
		}

		/// <summary>
		/// Does the serialization definition have support for processing values in parallel?
		/// </summary>
		public bool SupportsParallelProcessing
		{
			get => parallelProcessingSupport != null;
		}

		/// <summary>
		/// The lookup serialization definition.
		/// </summary>
		public new ILookupSerializationDefinition Definition
		{
			get => definition;
		}

		/// <summary>
		/// The type resolve serialization definition.
		/// </summary>
		public ILookupTypeResolveSupport TypeResolveDefinition
		{
			get => typeResolveSupport;
		}

		/// <summary>
		/// The required value serialization definition.
		/// </summary>
		public IRequiredValueSupport RequiredValueDefinition
		{
			get => requiredValueSupport;
		}

		/// <summary>
		/// The parallel processing serialization definition.
		/// </summary>
		public IParallelProcessingSupport ParallelProcessingDefinition
		{
			get => parallelProcessingSupport;
		}

		/// <summary>
		/// Are objects being processed required to be marked with a processing attribute?
		/// </summary>
		public bool RequiresMarking
		{
			get => requiresMarking;
		}

		public CustomObjectLookupProcessor(ILookupSerializationDefinition definition, bool requiresObjectMarking = true)
		: base(definition)
		{
			this.requiresMarking = requiresObjectMarking;
			this.definition = definition;
			this.typeResolveSupport = (definition is ILookupTypeResolveSupport typeResolveDefinition) ? typeResolveDefinition : null;
			this.requiredValueSupport = (definition is IRequiredValueSupport requiredValueDefinition) ? requiredValueDefinition : null;
			this.parallelProcessingSupport = (definition is IParallelProcessingSupport parallelProcessingDefinition) ? parallelProcessingDefinition : null;
		}

		/// <summary>
		/// Attempts to serialize the object to a dictionary-like data structure.
		/// </summary>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <param name="serializedResult">The serialized object.</param>
		/// <returns>True if the serialization is compatible and accepted, false otherwise.</returns>
		public override bool Serialize(object objectToSerialize, out object serializedResult)
		{
			if (objectToSerialize == null)
			{
				serializedResult = objectToSerialize;
				return true;
			}

			Type sourceType = objectToSerialize.GetType();
			if (RequiresMarking && !Attribute.IsDefined(sourceType, definition.LookupBasedClassMarkingAttribute))
			{
				serializedResult = null;
				return false;
			}

			InvokeOnSerializationCallback(objectToSerialize);
			serializedResult = Serialize(sourceType, objectToSerialize);
			InvokeOnSerializedCallback(objectToSerialize);
			return true;
		}

		/// <summary>
		/// Attempts to deserialize the dictionary-like data to a new instance of the target type.
		/// </summary>
		/// <param name="targetType">The target type to deserialize the given data.</param>
		/// <param name="dataToDeserialize">The data deserialize and apply to the result.</param>
		/// <param name="deserializedResult">The result unto which the data is applied.</param>
		/// <returns>True if deserialization is compatible and accepted, false otherwise.</returns>
		public override bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult)
		{
			targetType.ThrowIfNull(nameof(targetType));

			if (dataToDeserialize == null)
			{
				deserializedResult = null;
				return SerializationUtilities.IsNullableType(targetType);
			}
			else if (!(dataToDeserialize is IDictionary))
			{
				deserializedResult = null;
				return false;
			}

			Type instanceType = ResolveTypeForDeserialization(targetType, dataToDeserialize as IDictionary);
			if (RequiresMarking && !Attribute.IsDefined(instanceType, definition.LookupBasedClassMarkingAttribute))
			{
				deserializedResult = null;
				return false;
			}

			object targetInstance = SerializationUtilities.CreateInstance(instanceType);

			if (Deserialize(targetInstance, dataToDeserialize))
			{
				deserializedResult = targetInstance;
				return true;
			}
			else
			{
				deserializedResult = null;
				return false;
			}
		}

		/// <summary>
		/// Attempts to deserialize the list-like data to the given target.
		/// </summary>
		/// <param name="deserializationTarget">The object on which the data should be applied.</param>
		/// <param name="dataToDeserialize">The data to deserialize.</param>
		/// <returns>True if the deserialization is compatible and accepted, false otherwise.</returns>
		public bool Deserialize(object deserializationTarget, object dataToDeserialize)
		{
			deserializationTarget.ThrowIfNull(nameof(deserializationTarget));

			// If the source value is null, then there is little to do.
			if (dataToDeserialize == null)
			{
				return true;
			}
			else if (RequiresMarking && !Attribute.IsDefined(deserializationTarget.GetType(), definition.LookupBasedClassMarkingAttribute))
			{
				return false;
			}

			InvokeOnDeserializationCallback(deserializationTarget);
			Deserialize(deserializationTarget, dataToDeserialize as IDictionary);
			InvokeOnDeserializedCallback(deserializationTarget);
			return true;
		}

		private IDictionary Serialize(Type sourceType, object source)
		{
			ISerializableMember[] sourceMembers = SerializationUtilities.GetTypeMap(sourceType).GetSerializableMembers(definition.LookupBasedFieldAttribute);
			IDictionary processedValues = definition.CreateLookupInstance(sourceMembers.Length + 1); // Include capacity for type information.
			LookupCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(processedValues);
			object parallelLock = null;

			// Process the source key and value pairs.
			if (SupportsParallelProcessing && ParallelProcessingDefinition.Enabled && (sourceMembers.Length > 1))
			{
				parallelLock = new object();
				Parallel.ForEach(sourceMembers, SerializeMember);
			}
			else
			{
				Array.ForEach(sourceMembers, SerializeMember);
			}

			// Include type information, if available.
			if (SupportsTypeResolvement)
			{
				ITypeResolveParameter typeResolveAttr = ResolveTypeForSerialization(sourceType);
				if (typeResolveAttr != null)
				{
					object typeKey = ((typeResolveAttr is ILookupTypeResolveParameter lookupTypeResolveAttr) && (lookupTypeResolveAttr.KeyOverride != null)) ? lookupTypeResolveAttr.KeyOverride : typeResolveSupport.TypeResolveKey;
					typeKey = Serializer.Serialize(typeKey, definition);

					// If the data already contains a value for the processed key, then the type information is assumed
					// to be inferred from that value. Otherwise, add the processed type key as well.
					if (!processedValues.Contains(typeKey))
					{
						object typeValue = (typeResolveAttr.Value != null) ? typeResolveAttr.Value : typeResolveAttr.Target.Name;
						typeValue = Serializer.Serialize(typeValue, definition);
						SerializationUtilities.InsertInLookup(processedValues, collectionInfo, typeKey, typeValue);
					}
				}
			}

			return processedValues;

			void SerializeMember(ISerializableMember sourceMember)
			{
				object processedKey = Serializer.Serialize(GetKey(sourceMember), definition);
				object processedValue = Serializer.Serialize(sourceMember.GetValue(source), definition);

				if (parallelLock != null)
				{
					lock (parallelLock)
					{
						if (!processedValues.Contains(processedKey))
						{
							SerializationUtilities.InsertInLookup(processedValues, collectionInfo, processedKey, processedValue);
						}
					}
				}
				else
				{
					if (!processedValues.Contains(processedKey))
					{
						SerializationUtilities.InsertInLookup(processedValues, collectionInfo, processedKey, processedValue);
					}
				}
			}
		}

		private void Deserialize(object target, IDictionary source)
		{
			// Get all of the fields that would like to get their value filled in.
			ISerializationReflectionMap typeMap = SerializationUtilities.GetTypeMap(target.GetType());
			ISerializableMember[] targetMembers = typeMap.GetSerializableMembers(definition.LookupBasedFieldAttribute);
			object parallelLock = null;

			if (SupportsParallelProcessing && ParallelProcessingDefinition.Enabled && (source.Count > 1))
			{
				parallelLock = new object();
				Parallel.ForEach(targetMembers, DeserializeMember);
			}
			else
			{
				Array.ForEach(targetMembers, DeserializeMember);
			}

			void DeserializeMember(ISerializableMember targetMember)
			{
				object key = GetKey(targetMember);

				// See whether the source contains a value for this field.
				if (!source.Contains(key))
				{
					// Check whether this field is marked as required.
					if (SupportsRequiredValues && typeMap.IsMemberRequired(targetMember.Member, requiredValueSupport.RequiredAttributeType))
					{
						throw new SerializationException("The member '{0}' is marked as required on type {1} but is not present in the source.", targetMember.Member.Name, targetMember.Member.DeclaringType.Name);
					}
					else
					{
						Log.Warning("The source does not contain a value associated with key '{0}' for a target of type {1}.", key, target.GetType().Name);
						return;
					}
				}

				object result = Serializer.Deserialize(targetMember.MemberType, source[key], definition);

				if (result == null)
				{
					// If the value is not allowed to be null, then quit.
					if (SupportsRequiredValues &&
						typeMap.TryGetRequiredMemberInfo(targetMember.Member, requiredValueSupport.RequiredAttributeType, out IRequiredSerializableMember requiredAttrInfo) &&
						requiredAttrInfo.RequiredParameterAttribute.NullCheck)
					{
						throw new SerializationException("The member '{0}' is marked as required on type {1} but the value is null in the source.", targetMember.Member.Name, targetMember.Member.DeclaringType.Name);
					}

					Type memberType = targetMember.MemberType;
					result = memberType.IsValueType ? Activator.CreateInstance(memberType, true) : null;
				}

				if (parallelLock != null)
				{
					lock (parallelLock) targetMember.SetValue(target, result);
				}
				else
				{
					targetMember.SetValue(target, result);
				}
			}
		}

		private object GetKey(ISerializableMember member)
		{
			ILookupParameter lookupAttribute = member.Attribute as ILookupParameter;
			return (lookupAttribute.Key != null) ? lookupAttribute.Key : member.Member.Name;
		}

		private ITypeResolveParameter ResolveTypeForSerialization(Type sourceType)
		{
			if (!SupportsTypeResolvement)
			{
				return null;
			}

			ILookupTypeResolveSupport typeResolveImplementation = definition as ILookupTypeResolveSupport;
			ITypeResolveParameter[] typeResolveParameters = SerializationUtilities.
				GetTypeMap(sourceType).
				GetTypeResolveParameters(typeResolveImplementation.TypeResolveAttribute);

			return Array.Find(typeResolveParameters, tr => tr.Target == sourceType);
		}

		private Type ResolveTypeForDeserialization(Type targetType, IDictionary source)
		{
			if (!SupportsTypeResolvement)
			{
				return targetType;
			}

			LookupCollectionTypeInfo sourceCollectionInfo = SerializationUtilities.GetCollectionTypeInfo(source);
			ITypeResolveParameter[] typeResolveAttrs = SerializationUtilities.GetTypeMap(targetType).GetTypeResolveParameters(typeResolveSupport.TypeResolveAttribute);

			foreach (ITypeResolveParameter typeResolveAttr in typeResolveAttrs)
			{
				// Perform this filter as well as a way to stop the recursion.
				if (typeResolveAttr.Target == targetType)
				{
					continue;
				}

				Type resolvedTargetType = null;

				// Check if an override key type resolve parameter exists, and use it if a match could be made in the source data.
				if ((typeResolveAttr is ILookupTypeResolveParameter lookupTypeResolveAttr) && (lookupTypeResolveAttr.KeyOverride != null))
				{
					object processedKey = sourceCollectionInfo.PostProcessKey(Serializer.Serialize(lookupTypeResolveAttr.KeyOverride, Definition));
					resolvedTargetType = ResolveTypeForDeserialization(source, typeResolveAttr, processedKey);
				}

				// If no resolve type was found yet, attempt to find it using the regular attributes.
				if (resolvedTargetType == null)
				{
					object processedKey = sourceCollectionInfo.PostProcessKey(Serializer.Serialize(typeResolveSupport.TypeResolveKey, Definition));
					resolvedTargetType = ResolveTypeForDeserialization(source, typeResolveAttr, processedKey);
				}

				// Recursively find a more concrete type, if any.
				if (resolvedTargetType != null)
				{
					if (targetType.IsAssignableFrom(resolvedTargetType))
					{
						return ResolveTypeForDeserialization(resolvedTargetType, source);
					}
					else if (targetType.IsSubclassOf(resolvedTargetType))
					{
						continue;   // If we came back around to a base type again, then don't bother.
					}
					else
					{
						throw new SerializationException("The attribute of type {0}, defined on type {1} or its super types, is matched but cannot be assigned from instance of type {2}.", typeResolveAttr.GetType().Name, targetType.Name, typeResolveAttr.Target.Name);
					}
				}
			}

			return targetType;
		}

		private Type ResolveTypeForDeserialization(IDictionary source, ITypeResolveParameter typeResolveParam, object processedKey)
		{
			if ((processedKey == null) || !source.Contains(processedKey) || (source[processedKey] == null))
			{
				return null;
			}

			// Attempt to process the override value to the type of the value in the source data.
			object processedValue = (typeResolveParam.Value != null) ? typeResolveParam.Value : typeResolveParam.Target.Name;
			processedValue = SerializationUtilities.PostProcessValue(Serializer.Serialize(processedValue, Definition), source[processedKey].GetType());
			return source[processedKey].Equals(processedValue) ? typeResolveParam.Target : null;
		}
	}
}
