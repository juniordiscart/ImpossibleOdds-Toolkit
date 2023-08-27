using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using ImpossibleOdds.Serialization.Caching;

namespace ImpossibleOdds.Serialization.Processors
{

	/// <summary>
	/// A (de)serialization processor for dictionary-like data structures.
	/// </summary>
	public class LookupProcessor : ISerializationProcessor, IDeserializationToTargetProcessor
	{
		private readonly ILookupSerializationDefinition definition;

		public bool SupportsParallelProcessing => ParallelProcessingFeature != null;

		public IParallelProcessingFeature ParallelProcessingFeature { get; set; }

		ISerializationDefinition IProcessor.Definition => definition;

		ILookupSerializationDefinition Definition => definition;

		public LookupProcessor(ILookupSerializationDefinition definition)
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

			// Take the key-value pairs from the original source values and process
			// them individually to data that is accepted by the serialization definition
			// and is accepted by the underlying type restrictions of the result collection.
			IDictionary sourceValues = (IDictionary)objectToSerialize;
			IDictionary processedValues = definition.CreateLookupInstance(sourceValues.Count);
			LookupCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(processedValues);
			object parallelLock = null;

			if (SupportsParallelProcessing && ParallelProcessingFeature.Enabled && (sourceValues.Count > 1))
			{
				parallelLock = new object();
				Parallel.ForEach(sourceValues.Cast<DictionaryEntry>(), SerializeMember);
			}
			else
			{
				foreach (DictionaryEntry key in sourceValues)
				{
					SerializeMember(key);
				}
			}

			return processedValues;

			void SerializeMember(DictionaryEntry entry)
			{
				object processedKey = Serializer.Serialize(entry.Key, definition);
				object processedValue = Serializer.Serialize(entry.Value, definition);

				if (parallelLock != null)
				{
					lock (parallelLock) SerializationUtilities.InsertInLookup(processedValues, collectionInfo, processedKey, processedValue);
				}
				else
				{
					SerializationUtilities.InsertInLookup(processedValues, collectionInfo, processedKey, processedValue);
				}
			}
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			// If the value is null, we can just return already.
			if (dataToDeserialize == null)
			{
				return null;
			}

			IDictionary targetCollection = Activator.CreateInstance(targetType, true) as IDictionary;
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

			IDictionary sourceValues = (IDictionary)dataToDeserialize;
			IDictionary targetValues = (IDictionary)deserializationTarget;
			LookupCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(targetValues);
			object parallelLock = null;

			if (SupportsParallelProcessing && ParallelProcessingFeature.Enabled && (sourceValues.Count > 1))
			{
				parallelLock = new object();
				Parallel.ForEach(sourceValues.Cast<DictionaryEntry>(), DeserializeMember);
			}
			else
			{
				foreach (DictionaryEntry entry in sourceValues)
				{
					DeserializeMember(entry);
				}
			}

			void DeserializeMember(DictionaryEntry entry)
			{
				object processedKey = Serializer.Deserialize(collectionInfo.keyType, entry.Key, definition);
				object processedValue = Serializer.Deserialize(collectionInfo.valueType, entry.Value, definition);

				if (parallelLock != null)
				{
					lock (parallelLock) SerializationUtilities.InsertInLookup(targetValues, collectionInfo, processedKey, processedValue);
				}
				else
				{
					SerializationUtilities.InsertInLookup(targetValues, collectionInfo, processedKey, processedValue);
				}
			}
		}

		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			return
				(objectToSerialize == null) ||
				(objectToSerialize is IDictionary);
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			// Check if the target implements the general IDictionary interface, if not, we can just skip altogether.
			return
				typeof(IDictionary).IsAssignableFrom(targetType) &&
				((dataToDeserialize == null) || (dataToDeserialize is IDictionary));
		}
	}
}