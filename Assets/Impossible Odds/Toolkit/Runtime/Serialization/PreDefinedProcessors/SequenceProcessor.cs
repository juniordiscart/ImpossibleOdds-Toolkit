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
		public ISerializationDefinition Definition { get; }
		public ISequenceSerializationConfiguration Configuration { get; }
		public IParallelProcessingFeature ParallelProcessingFeature { get; set; }
		private bool ParallelProcessingEnabled => ParallelProcessingFeature is { Enabled: true };

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
			this.ThrowIfCantSerialize(objectToSerialize);
			
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
			ListCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(processedValues);

			if (ParallelProcessingEnabled && (sourceValues.Count > 1))
			{
				Parallel.For(0, sourceValues.Count, index =>
				{
					object processedValue = Serializer.Serialize(sourceValues[index], Definition);
					lock (processedValues) SerializationUtilities.InsertInList(processedValues, collectionInfo, index, processedValue);
				});
			}
			else
			{
				for (int index = 0; index < sourceValues.Count; ++index)
				{
					object processedValue = Serializer.Serialize(sourceValues[index], Definition);
					SerializationUtilities.InsertInList(processedValues, collectionInfo, index, processedValue);
				}
			}

			return processedValues;
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			this.ThrowIfCantDeserialize(targetType, dataToDeserialize);
			
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
			this.ThrowIfCantDeserializeToTarget(deserializationTarget, dataToDeserialize);
			
			// If there is nothing to do...
			if (dataToDeserialize == null)
			{
				return;
			}

			IList sourceValues = (IList)dataToDeserialize;
			IList targetValues = (IList)deserializationTarget;
			ListCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(targetValues);

			if (ParallelProcessingEnabled && (sourceValues.Count > 1))
			{
				Parallel.For(0, sourceValues.Count, index =>
				{
					object processedValue = collectionInfo.PostProcessValue(Serializer.Deserialize(collectionInfo.elementType, sourceValues[index], Definition));
					lock (targetValues) SerializationUtilities.InsertInList(targetValues, collectionInfo, index, processedValue);
				});
			}
			else
			{
				for (int index = 0; index < sourceValues.Count; ++index)
				{
					object processedValue = collectionInfo.PostProcessValue(Serializer.Deserialize(collectionInfo.elementType, sourceValues[index], Definition));
					SerializationUtilities.InsertInList(targetValues, collectionInfo, index, processedValue);
				}
			}
		}

		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			return objectToSerialize is null or IList;
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			// Check if the target implements the general IList interface, if not, we can just skip it altogether.
			return typeof(IList).IsAssignableFrom(targetType) && (dataToDeserialize is null or IList);
		}
		
		/// <inheritdoc />
		public bool CanDeserialize(object deserializationTarget, object dataToDeserialize)
		{
			return (deserializationTarget != null) && CanDeserialize(deserializationTarget.GetType(), dataToDeserialize);
		}
	}
}