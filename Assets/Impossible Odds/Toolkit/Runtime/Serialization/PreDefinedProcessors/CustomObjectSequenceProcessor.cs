namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using System.Threading.Tasks;
	using ImpossibleOdds.Serialization.Caching;

	/// <summary>
	/// A (de)serialization processor to process custom object to list-like data structures.
	/// </summary>
	public class CustomObjectSequenceProcessor : AbstractCustomObjectProcessor, ISerializationProcessor, IDeserializationToTargetProcessor
	{
		private readonly bool requiresMarking = false;
		private readonly IIndexSerializationDefinition definition = null;
		private readonly IIndexTypeResolveSupport typeResolveDefinition = null;
		private readonly IParallelProcessingSupport parallelProcessingSupport = null;

		/// <summary>
		/// Does the serialization definition have support for type resolve parameters?
		/// </summary>
		public bool SupportsTypeResolvement
		{
			get => typeResolveDefinition != null;
		}

		/// <summary>
		/// Does the serialization definition have support for processing values in parallel?
		/// </summary>
		public bool SupportsParallelProcessing
		{
			get => parallelProcessingSupport != null;
		}

		/// <summary>
		/// The type resolve serialization definition.
		/// </summary>
		public IIndexTypeResolveSupport TypeResolveDefinition
		{
			get => typeResolveDefinition;
		}

		/// <summary>
		/// The parallel processing serialization definition.
		/// </summary>
		public IParallelProcessingSupport ParallelProcessingDefinition
		{
			get => parallelProcessingSupport;
		}

		/// <summary>
		/// The index-based serialization definition.
		/// </summary>
		public new IIndexSerializationDefinition Definition
		{
			get => definition;
		}

		/// <summary>
		/// Are objects being processed required to be marked with a processing attribute?
		/// </summary>
		public bool RequiresMarking
		{
			get => requiresMarking;
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
			if (RequiresMarking && !Attribute.IsDefined(sourceType, definition.IndexBasedClassMarkingAttribute))
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
			targetType.ThrowIfNull(nameof(targetType));

			if (dataToDeserialize == null)
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
			if (RequiresMarking && !Attribute.IsDefined(instanceType, definition.IndexBasedClassMarkingAttribute))
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
			else if (RequiresMarking && !Attribute.IsDefined(deserializationTarget.GetType(), definition.IndexBasedClassMarkingAttribute))
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
			ISerializableMember[] sourceMembers = SerializationUtilities.GetTypeMap(sourceType).GetSerializableMembers(definition.IndexBasedFieldAttribute);
			object parallelLock = null;

			if (SupportsParallelProcessing && ParallelProcessingDefinition.Enabled && (sourceMembers.Length > 1))
			{
				parallelLock = new object();
				Parallel.ForEach(sourceMembers, SerializeMember);
			}
			else
			{
				Array.ForEach(sourceMembers, SerializeMember);
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

			void SerializeMember(ISerializableMember sourceMember)
			{
				object processedValue = Serializer.Serialize(sourceMember.GetValue(source), definition);
				if (parallelLock != null)
				{
					lock (parallelLock) SerializationUtilities.InsertInSequence(processedValues, collectionInfo, GetIndex(sourceMember), processedValue);
				}
				else
				{
					SerializationUtilities.InsertInSequence(processedValues, collectionInfo, GetIndex(sourceMember), processedValue);
				}
			}
		}

		private void Deserialize(object target, IList source)
		{
			// Get all of the fields that would like to get their value filled in
			ISerializableMember[] targetMembers = SerializationUtilities.GetTypeMap(target.GetType()).GetSerializableMembers(definition.IndexBasedFieldAttribute);
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
				int index = GetIndex(targetMember);

				// Check whether the source has such an index.
				if (source.Count <= index)
				{
					Log.Warning("The source does not contain a value at index '{0}' for a target of type {1}.", index, target.GetType().Name);
					return;
				}

				object result = Serializer.Deserialize(targetMember.MemberType, source[index], definition);

				// If the result is null, get the default value for that type.
				if (result == null)
				{
					result = SerializationUtilities.GetDefaultValue(targetMember.MemberType);
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

		private int GetIndex(ISerializableMember member)
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

			return Array.Find(SerializationUtilities.GetTypeMap(sourceType).GetTypeResolveParameters(TypeResolveDefinition.TypeResolveAttribute), tr => tr.Target == sourceType);
		}

		private Type ResolveTypeFromSequence(Type targetType, IList source)
		{
			if (!SupportsTypeResolvement || (source.Count <= typeResolveDefinition.TypeResolveIndex))
			{
				return targetType;
			}

			ITypeResolveParameter[] typeResolveAttrs = SerializationUtilities.GetTypeMap(targetType).GetTypeResolveParameters(typeResolveDefinition.TypeResolveAttribute);
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
				ISerializableMember[] members = SerializationUtilities.GetTypeMap(type).GetSerializableMembers(definition.IndexBasedFieldAttribute);
				foreach (ISerializableMember member in members)
				{
					maxIndex = Math.Max((member.Attribute as IIndexParameter).Index, maxIndex);
				}

				type = type.BaseType;
			}

			return maxIndex;
		}
	}
}
