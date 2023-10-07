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
		public bool ParallelProcessingEnabled => ParallelProcessingFeature is { Enabled: true};

		public IParallelProcessingFeature ParallelProcessingFeature { get; set; }

		public ISerializationDefinition Definition { get; }

		public ILookupSerializationConfiguration Configuration { get; }

		public LookupProcessor(ISerializationDefinition definition, ILookupSerializationConfiguration configuration)
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

			// Take the key-value pairs from the original source values and process
			// them individually to data that is accepted by the serialization definition
			// and is accepted by the underlying type restrictions of the result collection.
			IDictionary sourceValues = (IDictionary)objectToSerialize;
			IDictionary processedValues = Configuration.CreateLookupInstance(sourceValues.Count);
			LookupCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(processedValues);

			if (ParallelProcessingEnabled)
			{
				object parallelLock = new object();
				Parallel.ForEach(sourceValues.Cast<DictionaryEntry>(), entry =>
				{
					object processedKey = Serializer.Serialize(entry.Key, Definition);
					object processedValue = Serializer.Serialize(entry.Value, Definition);
					lock (parallelLock) SerializationUtilities.InsertInLookup(processedValues, collectionInfo, processedKey, processedValue);
				});
			}
			else
			{
				foreach (DictionaryEntry entry in sourceValues)
				{
					object processedKey = Serializer.Serialize(entry.Key, Definition);
					object processedValue = Serializer.Serialize(entry.Value, Definition);
					SerializationUtilities.InsertInLookup(processedValues, collectionInfo, processedKey, processedValue);
				}
			}

			return processedValues;
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

			if (ParallelProcessingEnabled && (sourceValues.Count > 1))
			{
				object parallelLock = new object();
				Parallel.ForEach(sourceValues.Cast<DictionaryEntry>(), entry =>
				{
					object processedKey = Serializer.Deserialize(collectionInfo.keyType, entry.Key, Definition);
					object processedValue = Serializer.Deserialize(collectionInfo.valueType, entry.Value, Definition);
					lock (parallelLock) SerializationUtilities.InsertInLookup(targetValues, collectionInfo, processedKey, processedValue);
				});
			}
			else
			{
				foreach (DictionaryEntry entry in sourceValues)
				{
					object processedKey = Serializer.Deserialize(collectionInfo.keyType, entry.Key, Definition);
					object processedValue = Serializer.Deserialize(collectionInfo.valueType, entry.Value, Definition);
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