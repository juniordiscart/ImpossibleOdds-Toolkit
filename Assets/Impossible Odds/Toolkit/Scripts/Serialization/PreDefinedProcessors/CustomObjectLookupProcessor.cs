namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using ImpossibleOdds.Serialization.Caching;

	/// <summary>
	/// A (de)serialization processor to process custom object to dictionary-like data structures.
	/// </summary>
	public class CustomObjectLookupProcessor : AbstractCustomObjectProcessor, IDeserializationToTargetProcessor
	{
		private bool requiresMarking = false;
		private ILookupSerializationDefinition definition = null;
		private ILookupTypeResolveSupport typeResolveSupport = null;
		private IRequiredValueSupport requiredValueSupport = null;

		/// <summary>
		/// Does the serialization definition have support for type resolve parameters?
		/// </summary>
		public bool SupportsTypeResolvement
		{
			get { return typeResolveSupport != null; }
		}

		/// <summary>
		/// Does the serialization definition have support for required values?
		/// </summary>
		public bool SupportsRequiredValues
		{
			get { return requiredValueSupport != null; }
		}

		/// <summary>
		/// The lookup serialization definition.
		/// </summary>
		public new ILookupSerializationDefinition Definition
		{
			get { return definition; }
		}

		/// <summary>
		/// The type resolve serialization definition.
		/// </summary>
		public ILookupTypeResolveSupport TypeResolveDefinition
		{
			get { return typeResolveSupport; }
		}

		/// <summary>
		/// The required value serialization definition.
		/// </summary>
		public IRequiredValueSupport RequiredValueDefinition
		{
			get { return requiredValueSupport; }
		}

		/// <summary>
		/// Are objects being processed required to be marked with a processing attribute?
		/// </summary>
		public bool RequiresMarking
		{
			get { return requiresMarking; }
		}

		public CustomObjectLookupProcessor(ILookupSerializationDefinition definition, bool requiresObjectMarking = true)
		: base(definition)
		{
			this.requiresMarking = requiresObjectMarking;
			this.definition = definition;
			this.typeResolveSupport = (definition is ILookupTypeResolveSupport) ? (definition as ILookupTypeResolveSupport) : null;
			this.requiredValueSupport = (definition is IRequiredValueSupport) ? (definition as IRequiredValueSupport) : null;
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
			if (RequiresMarking && !sourceType.IsDefined(definition.LookupBasedClassMarkingAttribute, true))
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
			if (targetType == null)
			{
				deserializedResult = null;
				return false;
			}
			else if (dataToDeserialize == null)
			{
				deserializedResult = null;
				return SerializationUtilities.IsNullableType(targetType);
			}
			else if (!(dataToDeserialize is IDictionary))
			{
				deserializedResult = null;
				return false;
			}

			Type instanceType = ResolveTypeFromLookup(targetType, dataToDeserialize as IDictionary);
			if (RequiresMarking && !instanceType.IsDefined(Definition.LookupBasedClassMarkingAttribute, true))
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
			else if (RequiresMarking && !deserializationTarget.GetType().IsDefined(definition.LookupBasedClassMarkingAttribute, true))
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
			IReadOnlyList<IMemberAttributeTuple> sourceMembers = GetTypeCache(sourceType).GetMembersWithAttribute(definition.LookupBasedFieldAttribute, SupportsRequiredValues ? requiredValueSupport.RequiredAttributeType : null);
			IDictionary processedValues = definition.CreateLookupInstance(sourceMembers.Count + 1); // Include capacity for type information.
			LookupCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(processedValues);

			// Process the source key and value pairs.
			foreach (IMemberAttributeTuple sourceMember in sourceMembers)
			{
				object processedKey = Serializer.Serialize(GetKey(sourceMember), definition);
				object processedValue = Serializer.Serialize(sourceMember.GetValue(source), definition);
				SerializationUtilities.InsertInLookup(processedValues, collectionInfo, processedKey, processedValue);
			}

			// Include type information, if available.
			if (SupportsTypeResolvement)
			{
				ITypeResolveParameter typeResolveAttr = ResolveTypeToLookup(sourceType);
				if (typeResolveAttr != null)
				{
					object processedTypeKey = Serializer.Serialize(typeResolveSupport.TypeResolveKey, definition);
					object processedTypeValue = Serializer.Serialize(typeResolveAttr.Value, definition);
					SerializationUtilities.InsertInLookup(processedValues, collectionInfo, processedTypeKey, processedTypeValue);
				}
			}

			return processedValues;
		}

		private void Deserialize(object target, IDictionary source)
		{
			// Get all of the fields that would like to get their value filled in.
			IReadOnlyList<IMemberAttributeTuple> targetMembers = GetTypeCache(target.GetType()).GetMembersWithAttribute(definition.LookupBasedFieldAttribute, SupportsRequiredValues ? requiredValueSupport.RequiredAttributeType : null);

			foreach (IMemberAttributeTuple targetMember in targetMembers)
			{
				object key = GetKey(targetMember);

				// See whether the source contains a value for this field.
				if (!source.Contains(key))
				{
					// Check whether this field is marked as required.
					if (SupportsRequiredValues && targetMember.IsRequiredParameter)
					{
						throw new SerializationException("The member '{0}' is marked as required on type {1} but is not present in the source.", targetMember.Member.Name, targetMember.Member.DeclaringType.Name);
					}
					else
					{
						Log.Warning("The source does not contain a value associated with key '{0}' for a target of type {1}.", key, target.GetType().Name);
						continue;
					}
				}

				object result = Serializer.Deserialize(targetMember.MemberType, source[key], definition);

				if (result == null)
				{
					// If the value is not allowed to be null, then quit.
					if (SupportsRequiredValues && targetMember.IsRequiredParameter && targetMember.RequiredParameter.NullCheck)
					{
						throw new SerializationException("The member '{0}' is marked as required on type {1} but the value is null in the source.", targetMember.Member.Name, targetMember.Member.DeclaringType.Name);
					}

					Type memberType = targetMember.MemberType;
					targetMember.SetValue(target, memberType.IsValueType ? Activator.CreateInstance(memberType, true) : null);
				}
				else
				{
					targetMember.SetValue(target, result);
				}
			}
		}

		private object GetKey(IMemberAttributeTuple member)
		{
			ILookupParameter lookupAttribute = member.Attribute as ILookupParameter;
			return (lookupAttribute.Key != null) ? lookupAttribute.Key : member.Member.Name;
		}

		private ITypeResolveParameter ResolveTypeToLookup(Type sourceType)
		{
			if (!SupportsTypeResolvement)
			{
				return null;
			}

			ILookupTypeResolveSupport typeResolveImplementation = definition as ILookupTypeResolveSupport;
			IReadOnlyList<ITypeResolveParameter> typeResolveAttributes = GetTypeCache(sourceType).GetTypeResolveParameters(typeResolveImplementation.TypeResolveAttribute);
			return typeResolveAttributes.FirstOrDefault(tr => tr.Target == sourceType);
		}

		private Type ResolveTypeFromLookup(Type targetType, IDictionary source)
		{
			if (!SupportsTypeResolvement || !source.Contains(typeResolveSupport.TypeResolveKey))
			{
				return targetType;
			}

			IReadOnlyList<ITypeResolveParameter> typeResolveAttrs = GetTypeCache(targetType).GetTypeResolveParameters(typeResolveSupport.TypeResolveAttribute);
			foreach (ITypeResolveParameter typeResolveAttr in typeResolveAttrs)
			{
				if (source[typeResolveSupport.TypeResolveKey].Equals(typeResolveAttr.Value))
				{
					if (targetType.IsAssignableFrom(typeResolveAttr.Target))
					{
						return typeResolveAttr.Target;
					}
					else
					{
						throw new SerializationException("The attribute of type {0}, defined on type {1} or its super types, is matched but cannot be assigned from instance of type {2}.", typeResolveAttr.GetType().Name, targetType.Name, typeResolveAttr.Target.Name);
					}
				}
			}

			return targetType;
		}
	}
}
