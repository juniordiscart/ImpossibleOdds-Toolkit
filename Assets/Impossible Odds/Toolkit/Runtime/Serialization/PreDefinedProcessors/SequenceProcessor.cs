using System;
using System.Collections;
using System.Threading.Tasks;
using ImpossibleOdds.Serialization.Caching;

namespace ImpossibleOdds.Serialization.Processors
{
	/// <summary>
	/// A (de)serialization processor for list-like data structures.
	/// </summary>
	public class SequenceProcessor : ISerializationProcessor, IDeserializationToTargetProcessor
	{
		public bool ParallelProcessingEnabled => ParallelProcessingFeature is { Enabled: true };

		public IParallelProcessingFeature ParallelProcessingFeature { get; set; }

		public ISerializationDefinition Definition { get; }

		public ISequenceSerializationConfiguration Configuration { get; }

		public SequenceProcessor(ISerializationDefinition definition, ISequenceSerializationConfiguration configuration)
		{
			definition.ThrowIfNull(nameof(definition));
			configuration.ThrowIfNull(nameof(configuration));
			Definition = definition;
			Configuration = configuration;
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
			IList processedValues = Configuration.CreateSequenceInstance(sourceValues.Count);
			SequenceCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(processedValues);

			if (ParallelProcessingEnabled && (sourceValues.Count > 1))
			{
				object parallelLock = new object();
				Parallel.For(0, sourceValues.Count, (int index) =>
				{
					object processedValue = Serializer.Serialize(sourceValues[index], Definition);
					lock (parallelLock) SerializationUtilities.InsertInSequence(processedValues, collectionInfo, index, processedValue);
				});
			}
			else
			{
				for (int index = 0; index < sourceValues.Count; ++index)
				{
					object processedValue = Serializer.Serialize(sourceValues[index], Definition);
					SerializationUtilities.InsertInSequence(processedValues, collectionInfo, index, processedValue);
				}
			}

			return processedValues;
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

			if (ParallelProcessingEnabled && (sourceValues.Count > 1))
			{
				object parallelLock = new object();
				Parallel.For(0, sourceValues.Count, (int index) =>
				{
					object processedValue = collectionInfo.PostProcessValue(Serializer.Deserialize(collectionInfo.elementType, sourceValues[index], Definition));
					lock (parallelLock) SerializationUtilities.InsertInSequence(targetValues, collectionInfo, index, processedValue);
				});
			}
			else
			{
				for (int index = 0; index < sourceValues.Count; ++index)
				{
					object processedValue = collectionInfo.PostProcessValue(Serializer.Deserialize(collectionInfo.elementType, sourceValues[index], Definition));
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