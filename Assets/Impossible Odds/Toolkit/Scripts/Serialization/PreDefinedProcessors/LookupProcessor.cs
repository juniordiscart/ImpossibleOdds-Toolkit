namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using ImpossibleOdds.Serialization.Caching;

	/// <summary>
	/// A (de)serialization processor for dictionary-like data structures.
	/// </summary>
	public class LookupProcessor : ISerializationProcessor, IDeserializationProcessor, IDeserializationToTargetProcessor
	{
		private ILookupSerializationDefinition definition;

		ISerializationDefinition IProcessor.Definition
		{
			get { return definition; }
		}

		ILookupSerializationDefinition Definition
		{
			get { return definition; }
		}

		public LookupProcessor(ILookupSerializationDefinition definition)
		{
			this.definition = definition;
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
			foreach (DictionaryEntry keyValuePair in sourceValues)
			{
				object processedKey = Serializer.Serialize(keyValuePair.Key, definition);
				object processedValue = Serializer.Serialize(keyValuePair.Value, definition);
				SerializationUtilities.InsertInLookup(processedValues, collectionInfo, processedKey, processedValue);
			}

			serializedResult = processedValues;
			return true;
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
			// Check if the target implements the general IDictionary interface, if not, we can just skip altogether.
			if ((targetType == null) || !typeof(IDictionary).IsAssignableFrom(targetType))
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
			foreach (DictionaryEntry dictionaryEntry in sourceValues)
			{
				object processedKey = Serializer.Deserialize(collectionInfo.keyType, dictionaryEntry.Key, definition);
				object processedValue = Serializer.Deserialize(collectionInfo.valueType, dictionaryEntry.Value, definition);
				SerializationUtilities.InsertInLookup(targetValues, collectionInfo, processedKey, processedValue);
			}

			return true;
		}
	}
}
