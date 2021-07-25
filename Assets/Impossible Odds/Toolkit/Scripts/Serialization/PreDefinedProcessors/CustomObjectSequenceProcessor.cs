namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using ImpossibleOdds.Serialization.Caching;

	/// <summary>
	/// A (de)serialization processor to process custom object to list-like data structures.
	/// </summary>
	public class CustomObjectSequenceProcessor : AbstractCustomObjectProcessor, ISerializationProcessor, IDeserializationToTargetProcessor
	{
		private bool requiresMarking = false;
		private IIndexSerializationDefinition definition = null;
		private IIndexTypeResolveSupport typeResolveDefinition = null;

		/// <summary>
		/// Does the serialization definition have support for type resolve parameters?
		/// </summary>
		public bool SupportsTypeResolvement
		{
			get { return typeResolveDefinition != null; }
		}

		/// <summary>
		/// The type resolve serialization definition.
		/// </summary>
		public IIndexTypeResolveSupport TypeResolveDefinition
		{
			get { return typeResolveDefinition; }
		}

		/// <summary>
		/// The index-based serialization definition.
		/// </summary>
		public new IIndexSerializationDefinition Definition
		{
			get { return definition; }
		}

		/// <summary>
		/// Are objects being processed required to be marked with a processing attribute?
		/// </summary>
		public bool RequiresMarking
		{
			get { return requiresMarking; }
		}

		public CustomObjectSequenceProcessor(IIndexSerializationDefinition definition, bool requiresMarking = true)
		: base(definition)
		{
			this.requiresMarking = requiresMarking;
			this.definition = definition;
			this.typeResolveDefinition = (definition is IIndexTypeResolveSupport) ? (definition as IIndexTypeResolveSupport) : null;
		}

		/// <summary>
		/// Attempts to serialize the object to a list-like data structure.
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
			if (RequiresMarking && !sourceType.GetTypeInfo().IsDefined(definition.IndexBasedClassMarkingAttribute, true))
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
		/// Attempts to deserialize the list-like data to a new instance of the target type.
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
			else if (!(dataToDeserialize is IList))
			{
				deserializedResult = null;
				return false;
			}

			// Create instance
			Type instanceType = ResolveTypeFromSequence(targetType, dataToDeserialize as IList);
			if (RequiresMarking && !instanceType.IsDefined(Definition.IndexBasedClassMarkingAttribute, true))
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
			else if (RequiresMarking && !deserializationTarget.GetType().IsDefined(definition.IndexBasedClassMarkingAttribute, true))
			{
				return false;
			}

			InvokeOnDeserializationCallback(deserializationTarget);
			Deserialize(deserializationTarget, dataToDeserialize as IList);
			InvokeOnDeserializedCallback(deserializationTarget);
			return true;
		}

		private IList Serialize(Type sourceType, object source)
		{
			// Check how many elements need to be stored.
			int nrOfElements = Math.Max((SupportsTypeResolvement ? typeResolveDefinition.TypeResolveIndex : 0), GetMaxDefinedIndex(source.GetType()));
			nrOfElements += (nrOfElements >= 0) ? 1 : 0;    // Increase by 1 because of 0-based index.

			// Create the collection of result values.
			IList processedValues = definition.CreateSequenceInstance(nrOfElements);
			SequenceCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(processedValues);

			// Get the members that will be inserted in the collection and order them by their index.
			IReadOnlyList<IMemberAttributeTuple> sourceMembers = GetTypeCache(sourceType).GetMembersWithAttribute(definition.IndexBasedFieldAttribute);
			foreach (IMemberAttributeTuple sourceMember in sourceMembers)
			{
				object processedValue = Serializer.Serialize(sourceMember.GetValue(source), definition);
				SerializationUtilities.InsertInSequence(processedValues, collectionInfo, GetIndex(sourceMember), processedValue);
			}

			// Add the type resolve value.
			if (SupportsTypeResolvement)
			{
				ITypeResolveParameter typeResolveAttr = ResolveTypeToSequence(sourceType);
				if (typeResolveAttr != null)
				{
					object processedTypeValue = Serializer.Serialize(typeResolveAttr.Value, definition);
					SerializationUtilities.InsertInSequence(processedValues, collectionInfo, typeResolveDefinition.TypeResolveIndex, processedTypeValue);
				}
			}

			return processedValues;
		}

		private void Deserialize(object target, IList source)
		{
			// Get all of the fields that would like to get their value filled in
			IReadOnlyList<IMemberAttributeTuple> targetMembers = GetTypeCache(target.GetType()).GetMembersWithAttribute(definition.IndexBasedFieldAttribute);

			foreach (IMemberAttributeTuple targetMember in targetMembers)
			{
				int index = GetIndex(targetMember);

				// Check whether the source has such an index.
				if (source.Count <= index)
				{
					Log.Warning("The source does not contain a value at index '{0}' for a target of type {1}.", index, target.GetType().Name);
					continue;
				}

				object result = Serializer.Deserialize(targetMember.MemberType, source[index], definition);

				// If the result is null, get the default value for that type.
				if (result == null)
				{
					result = SerializationUtilities.GetDefaultValue(targetMember.MemberType);
				}

				targetMember.SetValue(target, result);
			}
		}

		private int GetIndex(IMemberAttributeTuple member)
		{
			if (member.Attribute is IIndexParameter indexParameter)
			{
				return indexParameter.Index;
			}
			else
			{
				throw new SerializationException(
						"Member {0} of type {1} is defined to be serialized using attribute of type {2}, but it does not implement the {3} interface.",
						member.Member.Name,
						member.Member.DeclaringType.Name,
						member.Attribute.GetType().Name,
						typeof(IIndexParameter).Name);
			}
		}

		private ITypeResolveParameter ResolveTypeToSequence(Type sourceType)
		{
			if (!SupportsTypeResolvement)
			{
				return null;
			}

			IIndexTypeResolveSupport typeResolveImplementation = definition as IIndexTypeResolveSupport;
			IEnumerable<ITypeResolveParameter> typeResolveAttributes = GetTypeCache(sourceType).GetTypeResolveParameters(typeResolveImplementation.TypeResolveAttribute);
			return typeResolveAttributes.FirstOrDefault(tr => tr.Target == sourceType);
		}

		private Type ResolveTypeFromSequence(Type targetType, IList source)
		{
			if (!SupportsTypeResolvement || (source.Count <= typeResolveDefinition.TypeResolveIndex))
			{
				return targetType;
			}

			IEnumerable<ITypeResolveParameter> typeResolveAttrs = GetTypeCache(targetType).GetTypeResolveParameters(typeResolveDefinition.TypeResolveAttribute);
			foreach (ITypeResolveParameter typeResolveAttr in typeResolveAttrs)
			{
				if (source[typeResolveDefinition.TypeResolveIndex].Equals(typeResolveAttr.Value))
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

		private int GetMaxDefinedIndex(Type type)
		{
			int maxIndex = int.MinValue;
			while ((type != null) && (type != typeof(object)))
			{
				IReadOnlyList<IMemberAttributeTuple> members = GetTypeCache(type).GetMembersWithAttribute(definition.IndexBasedFieldAttribute);
				foreach (IMemberAttributeTuple member in members)
				{
					maxIndex = Math.Max((member.Attribute as IIndexParameter).Index, maxIndex);
				}

				type = type.BaseType;
			}

			return maxIndex;
		}
	}
}
