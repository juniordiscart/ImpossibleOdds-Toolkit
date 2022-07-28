namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using System.Linq;
	using System.Threading.Tasks;
	using ImpossibleOdds.Serialization.Caching;

	/// <summary>
	/// A (de)serialization processor for dictionary-like data structures.
	/// </summary>
	public class LookupProcessor : ISerializationProcessor, IDeserializationProcessor, IDeserializationToTargetProcessor
	{
		private readonly ILookupSerializationDefinition definition;
		private readonly IParallelProcessingSupport parallelProcessingSupport = null;

		/// <summary>
		/// Does the serialization definition have support for processing values in parallel?
		/// </summary>
		public bool SupportsParallelProcessing
		{
			get => parallelProcessingSupport != null;
		}

		/// <summary>
		/// The parallel processing serialization definition.
		/// </summary>
		public IParallelProcessingSupport ParallelProcessingDefinition
		{
			get => parallelProcessingSupport;
		}

		ISerializationDefinition IProcessor.Definition
		{
			get => definition;
		}

		ILookupSerializationDefinition Definition
		{
			get => definition;
		}

		public LookupProcessor(ILookupSerializationDefinition definition)
		{
			this.definition = definition;
			this.parallelProcessingSupport = (definition is IParallelProcessingSupport parallelProcessingDefinition) ? parallelProcessingDefinition : null;
		}

		/// <summary>
		/// Attempts to serialize the given dictionary-like data structure to a data structure that is supported by the serialization definition. Each key-value pair is serialized individually as well.
		/// </summary>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <param name="serializedResult">The serialized object.</param>
		/// <returns>True if the serialization is compatible and accepted, false otherwise.</returns>
		public bool Serialize(object objectToSerialize, out object serializedResult)
		{
			// Accept null values.
			if ((objectToSerialize == null))
			{
				serializedResult = null;
				return true;
			}

			if (!(objectToSerialize is IDictionary))
			{
				serializedResult = null;
				return false;
			}

			// Take the key-value pairs from the original source values and process
			// them individually to data that is accepted by the serialization definition
			// and is accepted by the underlying type restrictions of the result collection.
			IDictionary sourceValues = objectToSerialize as IDictionary;
			IDictionary processedValues = definition.CreateLookupInstance(sourceValues.Count);
			LookupCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(processedValues);
			object parallelLock = null;

			if (SupportsParallelProcessing && ParallelProcessingDefinition.Enabled && (sourceValues.Count > 1))
			{
				parallelLock = new object();
				Parallel.ForEach<DictionaryEntry>(sourceValues.Cast<DictionaryEntry>(), SerializeMember);
			}
			else
			{
				foreach (DictionaryEntry key in sourceValues)
				{
					SerializeMember(key);
				}
			}

			serializedResult = processedValues;
			return true;

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

		/// <summary>
		/// Attempts to deserialize the given dictionary-like data structure and apply the data onto a new instance of the target type. Each key-valye pair is deserialized individually as well.
		/// </summary>
		/// <param name="targetType">The target type to deserialize the given data.</param>
		/// <param name="dataToDeserialize">The data deserialize and apply to the result.</param>
		/// <param name="deserializedResult">The result unto which the data is applied.</param>
		/// <returns>True if deserialization is compatible and accepted, false otherwise.</returns>
		public bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult)
		{
			targetType.ThrowIfNull(nameof(targetType));

			// Check if the target implements the general IDictionary interface, if not, we can just skip altogether.
			if (!typeof(IDictionary).IsAssignableFrom(targetType))
			{
				deserializedResult = null;
				return false;
			}

			// If the value is null, we can just assign it.
			if (dataToDeserialize == null)
			{
				deserializedResult = null;
				return true;
			}

			if (!(dataToDeserialize is IDictionary))
			{
				throw new SerializationException("The source value is expected to implement the {0} interface to process to target type {1}.", typeof(IDictionary), targetType.Name);
			}

			IDictionary targetCollection = Activator.CreateInstance(targetType, true) as IDictionary;
			if (!Deserialize(targetCollection, dataToDeserialize))
			{
				throw new SerializationException("Unexpected failure to process source value of type {0} to target collection of type {1}.", dataToDeserialize.GetType().Name, targetType.Name);
			}

			deserializedResult = targetCollection;
			return true;
		}

		/// <summary>
		/// Attempts to deserialize the given dictionary-like data structure and apply the data onto the given target. Each key-valye pair is deserialized individually as well.
		/// </summary>
		/// <param name="deserializationTarget">The object on which the data should be applied.</param>
		/// <param name="dataToDeserialize">The data to deserialize.</param>
		/// <returns>True if the deserialization is compatible and accepted, false otherwise.</returns>
		public bool Deserialize(object deserializationTarget, object dataToDeserialize)
		{
			if ((deserializationTarget == null) || !(deserializationTarget is IDictionary))
			{
				return false;
			}

			// If there is nothing to do...
			if (dataToDeserialize == null)
			{
				return true;
			}

			if (!(dataToDeserialize is IDictionary))
			{
				throw new SerializationException("The source value is expected to implement the {0} interface to process to a target instance of type {1}.", typeof(IDictionary), deserializationTarget.GetType().Name);
			}

			IDictionary sourceValues = dataToDeserialize as IDictionary;
			IDictionary targetValues = deserializationTarget as IDictionary;
			LookupCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(targetValues);
			object parallelLock = null;

			if (SupportsParallelProcessing && ParallelProcessingDefinition.Enabled && (sourceValues.Count > 1))
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

			return true;

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
	}
}
