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

		public bool SupportsTypeResolvement => typeResolveSupport != null;

		public bool SupportsRequiredValues => RequiredValueFeature != null;

		public bool SupportsParallelProcessing => ParallelProcessingFeature != null;

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

		public IRequiredValueFeature RequiredValueFeature { get; set; }

		public IParallelProcessingFeature ParallelProcessingFeature { get; set; }

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
		}

		/// <inheritdoc />
		public override object Serialize(object objectToSerialize)
		{
			if (objectToSerialize == null)
			{
				return null;
			}

			InvokeOnSerializationCallback(objectToSerialize);
			object serializedResult = Serialize(objectToSerialize.GetType(), objectToSerialize);
			InvokeOnSerializedCallback(objectToSerialize);
			return serializedResult;
		}

		/// <inheritdoc />
		public override object Deserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			Type instanceType = ResolveTypeForDeserialization(targetType, dataToDeserialize as IDictionary);
			object targetInstance = SerializationUtilities.CreateInstance(instanceType);
			Deserialize(targetInstance, dataToDeserialize);
			return targetInstance;
		}

		/// <inheritdoc />
		public virtual void Deserialize(object deserializationTarget, object dataToDeserialize)
		{
			deserializationTarget.ThrowIfNull(nameof(deserializationTarget));

			// If the source value is null, then there is little to do.
			if (dataToDeserialize == null)
			{
				return;
			}

			InvokeOnDeserializationCallback(deserializationTarget);
			Deserialize(deserializationTarget, dataToDeserialize as IDictionary);
			InvokeOnDeserializedCallback(deserializationTarget);
		}

		/// <inheritdoc />
		public override bool CanSerialize(object objectToSerialize)
		{
			// Either the object is null - which is accepted,
			// or the object does not require class marking,
			// or it requires class marking and it is class marked.
			return
				(objectToSerialize == null) ||
				!RequiresMarking ||
				Attribute.IsDefined(objectToSerialize.GetType(), definition.LookupBasedClassMarkingAttribute);
		}

		/// <inheritdoc />
		public override bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			if (dataToDeserialize == null)
			{
				return SerializationUtilities.IsNullableType(targetType);
			}

			if (!(dataToDeserialize is IDictionary))
			{
				return false;
			}

			Type instanceType = ResolveTypeForDeserialization(targetType, (IDictionary)dataToDeserialize);
			return !RequiresMarking || Attribute.IsDefined(instanceType, definition.LookupBasedClassMarkingAttribute);
		}

		private IDictionary Serialize(Type sourceType, object source)
		{
			ISerializableMember[] sourceMembers = SerializationUtilities.GetTypeMap(sourceType).GetSerializableMembers(definition.LookupBasedFieldAttribute);
			IDictionary processedValues = definition.CreateLookupInstance(sourceMembers.Length + 1); // Include capacity for type information.
			LookupCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(processedValues);
			object parallelLock = null;

			// Process the source key and value pairs.
			if (SupportsParallelProcessing && ParallelProcessingFeature.Enabled && (sourceMembers.Length > 1))
			{
				parallelLock = new object();
				Parallel.ForEach(sourceMembers, SerializeMember);
			}
			else
			{
				Array.ForEach(sourceMembers, SerializeMember);
			}

			// Include type information, if available.
			if (!SupportsTypeResolvement)
			{
				return processedValues;
			}

			// If no type resolve parameters exist, then don't include it and quit early.
			ITypeResolveParameter typeResolveAttr = ResolveTypeForSerialization(sourceType);
			if (typeResolveAttr == null)
			{
				return processedValues;
			}

			object typeKey = ((typeResolveAttr is ILookupTypeResolveParameter lookupTypeResolveAttr) && (lookupTypeResolveAttr.KeyOverride != null)) ? lookupTypeResolveAttr.KeyOverride : typeResolveSupport.TypeResolveKey;
			typeKey = Serializer.Serialize(typeKey, definition);

			// If the data already contains a value for the processed key, then the type information is assumed
			// to be inferred from that value. Otherwise, add the processed type key as well.
			if (processedValues.Contains(typeKey))
			{
				return processedValues;
			}

			object typeValue = typeResolveAttr.Value ?? typeResolveAttr.Target.Name;
			typeValue = Serializer.Serialize(typeValue, definition);
			SerializationUtilities.InsertInLookup(processedValues, collectionInfo, typeKey, typeValue);
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

			if (SupportsParallelProcessing && ParallelProcessingFeature.Enabled && (source.Count > 1))
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
					if (SupportsRequiredValues && typeMap.IsMemberRequired(targetMember.Member, RequiredValueFeature.RequiredValueAttributeType))
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
						typeMap.TryGetRequiredMemberInfo(targetMember.Member, RequiredValueFeature.RequiredValueAttributeType, out IRequiredSerializableMember requiredAttrInfo) &&
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
			return lookupAttribute.Key ?? member.Member.Name;
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
			object processedValue = typeResolveParam.Value ?? typeResolveParam.Target.Name;
			processedValue = SerializationUtilities.PostProcessValue(Serializer.Serialize(processedValue, Definition), source[processedKey].GetType());
			return source[processedKey].Equals(processedValue) ? typeResolveParam.Target : null;
		}
	}
}