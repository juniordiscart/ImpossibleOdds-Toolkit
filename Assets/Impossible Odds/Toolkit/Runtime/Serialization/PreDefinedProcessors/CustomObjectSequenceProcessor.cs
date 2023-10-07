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
		public bool SupportsTypeResolution => TypeResolutionFeature != null;

		public bool SupportsParallelProcessing => ParallelProcessingFeature is { Enabled: true };

		public ISequenceTypeResolutionFeature TypeResolutionFeature { get; set; }

		public IParallelProcessingFeature ParallelProcessingFeature { get; set; }

		public ISequenceSerializationConfiguration Configuration { get; }

		public bool RequiresMarking { get; }

		public CustomObjectSequenceProcessor(ISerializationDefinition definition, ISequenceSerializationConfiguration configuration, bool requiresMarking = true)
		: base(definition)
		{
			configuration.ThrowIfNull(nameof(configuration));
			RequiresMarking = requiresMarking;
			Configuration = configuration;
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

			Type instanceType = ResolveTypeFromSequence(targetType, (IList)dataToDeserialize);
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
			Deserialize(deserializationTarget, (IList)dataToDeserialize);
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
				Attribute.IsDefined(objectToSerialize.GetType(), Configuration.TypeMarkingAttribute);
		}

		/// <inheritdoc />
		public override bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			if (dataToDeserialize == null)
			{
				return SerializationUtilities.IsNullableType(targetType);
			}

			if (!(dataToDeserialize is IList list))
			{
				return false;
			}

			Type instanceType = ResolveTypeFromSequence(targetType, list);
			return !RequiresMarking || Attribute.IsDefined(instanceType, Configuration.TypeMarkingAttribute);
		}

		private IList Serialize(Type sourceType, object source)
		{
			// Check how many elements need to be stored.
			int maxIndex = Math.Max((SupportsTypeResolution ? TypeResolutionFeature.TypeResolutionIndex : 0), Configuration.GetMaxDefinedIndex(sourceType));
			maxIndex += (maxIndex >= 0) ? 1 : 0;    // Increase by 1 because of 0-based index.

			// Create the collection of result values.
			IList processedValues = Configuration.CreateSequenceInstance(maxIndex);
			SequenceCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(processedValues);

			// Get the members that will be inserted in the collection. Filter out duplicate indices.
			ISerializableMember[] sourceMembers = SerializationUtilities.GetTypeMap(sourceType).GetUniqueSerializableMembers(Configuration.MemberAttribute);

			if (SupportsParallelProcessing)
			{
				object parallelLock = new object();
				Parallel.ForEach(sourceMembers, sourceMember =>
				{
					object processedValue = Serializer.Serialize(sourceMember.GetValue(source), Definition);
					lock (parallelLock) SerializationUtilities.InsertInSequence(processedValues, collectionInfo, Configuration.GetIndex(sourceMember), processedValue);
				});
			}
			else
			{
				foreach (ISerializableMember sourceMember in sourceMembers)
				{
					object processedValue = Serializer.Serialize(sourceMember.GetValue(source), Definition);
					SerializationUtilities.InsertInSequence(processedValues, collectionInfo, Configuration.GetIndex(sourceMember), processedValue);
				}
			}

			if (SupportsTypeResolution)
			{
				TypeResolutionFeature.InsertTypeInData(sourceType, processedValues, Definition);
			}

			return processedValues;
		}

		private void Deserialize(object target, IList source)
		{
			// Get all of the fields that would like to get their value filled in.
			ISerializableMember[] targetMembers = SerializationUtilities.GetTypeMap(target.GetType()).GetSerializableMembers(Configuration.MemberAttribute);

			if (SupportsParallelProcessing)
			{
				object parallelLock = new object();
				Parallel.ForEach(targetMembers, member =>
				{
					int index = Configuration.GetIndex(member);
					if (source.Count > index)
					{
						object result = DeserializeMember(member, index);
						lock (parallelLock) member.SetValue(target, result);
					}
				});
			}
			else
			{
				Array.ForEach(targetMembers, member =>
				{
					int index = Configuration.GetIndex(member);
					if (source.Count > index)
					{
						member.SetValue(target, DeserializeMember(member, index));
					}
				});
			}

			return;

			object DeserializeMember(ISerializableMember targetMember, int index)
			{
				// If the result is null, get the default value for that type.
				return
					Serializer.Deserialize(targetMember.MemberType, source[index], Definition) ??
					SerializationUtilities.GetDefaultValue(targetMember.MemberType);
			}
		}

		private Type ResolveTypeFromSequence(Type targetType, IList source)
		{
			if (!SupportsTypeResolution || (source.Count <= TypeResolutionFeature.TypeResolutionIndex))
			{
				return targetType;
			}

			ITypeResolutionParameter[] typeResolveAttrs = SerializationUtilities.GetTypeMap(targetType).GetTypeResolveParameters(TypeResolutionFeature.TypeResolutionAttribute);
			foreach (ITypeResolutionParameter typeResolveAttr in typeResolveAttrs)
			{
				if (!source[TypeResolutionFeature.TypeResolutionIndex].Equals(typeResolveAttr.Value))
				{
					continue;
				}

				if (targetType.IsAssignableFrom(typeResolveAttr.Target))
				{
					return typeResolveAttr.Target;
				}

				throw new SerializationException($"The attribute of type {typeResolveAttr.GetType().Name}, defined on type {targetType.Name} or its super types, is matched but cannot be assigned from instance of type {typeResolveAttr.Target.Name}.");
			}

			return targetType;
		}
	}
}