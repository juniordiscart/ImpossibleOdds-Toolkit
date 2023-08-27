namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using System.Threading.Tasks;
	using ImpossibleOdds.Serialization.Caching;

	/// <summary>
	/// A (de)serialization processor for list-like data structures.
	/// </summary>
	public class SequenceProcessor : ISerializationProcessor, IDeserializationProcessor, IDeserializationToTargetProcessor
	{
		private readonly IIndexSerializationDefinition definition;

		public bool SupportsParallelProcessing => ParallelProcessingFeature != null;
		public IParallelProcessingFeature ParallelProcessingFeature { get; set; }

		ISerializationDefinition IProcessor.Definition
		{
			get => definition;
		}

		IIndexSerializationDefinition Definition
		{
			get => definition;
		}

		public SequenceProcessor(IIndexSerializationDefinition definition)
		{
			this.definition = definition;
		}

		/// <inheritdoc />
		public virtual object Serialize(object objectToSerialize)
		{
			// Accept null values.
			if ((objectToSerialize == null))
			{
				return null;
			}

			// Take the values from the original source values and process
			// them individually to data that is accepted by the serialization definition
			// and is accepted by the underlying type restrictions of the result collection.
			IList sourceValues = (IList)objectToSerialize;
			IList processedValues = definition.CreateSequenceInstance(sourceValues.Count);
			SequenceCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(processedValues);
			object parallelLock = null;

			if (SupportsParallelProcessing && ParallelProcessingFeature.Enabled && (sourceValues.Count > 1))
			{
				parallelLock = new object();
				Parallel.For(0, sourceValues.Count, SerializeMember);
			}
			else
			{
				for (int i = 0; i < sourceValues.Count; ++i)
				{
					SerializeMember(i);
				}
			}

			return processedValues;

			void SerializeMember(int index)
			{
				object processedValue = Serializer.Serialize(sourceValues[index], definition);
				if (parallelLock != null)
				{
					lock (parallelLock) SerializationUtilities.InsertInSequence(processedValues, collectionInfo, index, processedValue);
				}
				else
				{
					SerializationUtilities.InsertInSequence(processedValues, collectionInfo, index, processedValue);
				}
			}
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			// If the value is null, we can just assign it.
			if (dataToDeserialize == null)
			{
				return null;
			}

			// Arrays are treated differently.
			IList targetCollection =
				targetType.IsArray ?
				Array.CreateInstance(targetType.GetElementType(), ((IList)dataToDeserialize).Count) :
				(IList)Activator.CreateInstance(targetType, true);

			Deserialize(targetCollection, dataToDeserialize);
			return targetCollection;
		}

		/// <inheritdoc />
		public virtual void Deserialize(object deserializationTarget, object dataToDeserialize)
		{
			// If there is nothing to do...
			if (dataToDeserialize == null)
			{
				return;
			}

			IList sourceValues = (IList)dataToDeserialize;
			IList targetValues = (IList)deserializationTarget;
			SequenceCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(targetValues);
			object parallelLock = null;

			if (SupportsParallelProcessing && ParallelProcessingFeature.Enabled && (sourceValues.Count > 1))
			{
				parallelLock = new object();
				Parallel.For(0, sourceValues.Count, DeserializeMember);
			}
			else
			{
				for (int i = 0; i < sourceValues.Count; ++i)
				{
					DeserializeMember(i);
				}
			}

			void DeserializeMember(int index)
			{
				object processedValue = Serializer.Deserialize(collectionInfo.elementType, sourceValues[index], definition);
				processedValue = collectionInfo.PostProcessValue(processedValue);
				if (parallelLock != null)
				{
					lock (parallelLock) SerializationUtilities.InsertInSequence(targetValues, collectionInfo, index, processedValue);
				}
				else
				{
					SerializationUtilities.InsertInSequence(targetValues, collectionInfo, index, processedValue);
				}
			}
		}

		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			return
				(objectToSerialize == null) ||
				(objectToSerialize is IList);
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			// Check if the target implements the general IList interface, if not, we can just skip it altogether.
			return
				typeof(IList).IsAssignableFrom(targetType) &&
				((dataToDeserialize == null) || (dataToDeserialize is IList));
		}
	}
}