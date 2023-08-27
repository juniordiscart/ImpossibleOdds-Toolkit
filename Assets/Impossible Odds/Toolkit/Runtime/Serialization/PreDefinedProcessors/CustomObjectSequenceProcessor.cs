using System;
using System.Collections;
using System.Threading.Tasks;
using ImpossibleOdds.Serialization.Caching;

namespace ImpossibleOdds.Serialization.Processors
{

	/// <summary>
	/// A (de)serialization processor to process custom object to list-like data structures.
	/// </summary>
	public class CustomObjectSequenceProcessor : AbstractCustomObjectProcessor, ISerializationProcessor, IDeserializationToTargetProcessor
	{
		private readonly bool requiresMarking = false;
		private readonly IIndexSerializationDefinition definition = null;
		private readonly IIndexTypeResolveSupport typeResolveDefinition = null;

		public bool SupportsTypeResolvement => typeResolveDefinition != null;

		public bool SupportsParallelProcessing => ParallelProcessingFeature != null;

		public IIndexTypeResolveSupport TypeResolveDefinition => typeResolveDefinition;

		public IParallelProcessingFeature ParallelProcessingFeature { get; set; }

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

			Type instanceType = ResolveTypeFromSequence(targetType, dataToDeserialize as IList);
			object targetInstance = SerializationUtilities.CreateInstance(instanceType);
			Deserialize(targetInstance, dataToDeserialize);
			return targetInstance;
		}

		/// <inheritdoc />
		public void Deserialize(object deserializationTarget, object dataToDeserialize)
		{
			deserializationTarget.ThrowIfNull(nameof(deserializationTarget));

			// If the source value is null, then there is little to do.
			if (dataToDeserialize == null)
			{
				return;
			}

			InvokeOnDeserializationCallback(deserializationTarget);
			Deserialize(deserializationTarget, dataToDeserialize as IList);
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
				Attribute.IsDefined(objectToSerialize.GetType(), definition.IndexBasedClassMarkingAttribute);
		}

		/// <inheritdoc />
		public override bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			if (dataToDeserialize == null)
			{
				return SerializationUtilities.IsNullableType(targetType);
			}
			else if (!(dataToDeserialize is IList))
			{
				return false;
			}

			Type instanceType = ResolveTypeFromSequence(targetType, dataToDeserialize as IList);
			return !RequiresMarking || Attribute.IsDefined(instanceType, definition.IndexBasedClassMarkingAttribute);
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

			if (SupportsParallelProcessing && ParallelProcessingFeature.Enabled && (sourceMembers.Length > 1))
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
				int index = GetIndex(targetMember);

				// Check whether the source has such an index.
				if (source.Count <= index)
				{
					Log.Warning("The source does not contain a value at index '{0}' for a target of type {1}.", index, target.GetType().Name);
					return;
				}

				// If the result is null, get the default value for that type.
				object result =
					Serializer.Deserialize(targetMember.MemberType, source[index], definition) ??
					SerializationUtilities.GetDefaultValue(targetMember.MemberType);

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
			throw new SerializationException(
					"Member {0} of type {1} is defined to be serialized using attribute of type {2}, but it does not implement the {3} interface.",
					member.Member.Name,
					member.Member.DeclaringType.Name,
					member.Attribute.GetType().Name,
					nameof(IIndexParameter));
		}

		private ITypeResolveParameter ResolveTypeToSequence(Type sourceType)
		{
			return
				SupportsTypeResolvement ? Array.Find(SerializationUtilities.GetTypeMap(sourceType).GetTypeResolveParameters(TypeResolveDefinition.TypeResolveAttribute), tr => tr.Target == sourceType) : null;
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
					maxIndex = Math.Max(((IIndexParameter)member.Attribute).Index, maxIndex);
				}

				type = type.BaseType;
			}

			return maxIndex;
		}
	}
}