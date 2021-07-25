namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using ImpossibleOdds.Serialization.Caching;

	/// <summary>
	/// A (de)serialization processor for list-like data structures.
	/// </summary>
	public class SequenceProcessor : ISerializationProcessor, IDeserializationProcessor, IDeserializationToTargetProcessor
	{
		private IIndexSerializationDefinition definition;

		ISerializationDefinition IProcessor.Definition
		{
			get { return definition; }
		}

		IIndexSerializationDefinition Definition
		{
			get { return definition; }
		}

		public SequenceProcessor(IIndexSerializationDefinition definition)
		{
			this.definition = definition;
		}

		/// <summary>
		/// Attempts to serialize the object to another sequence in which all individual elements are processed by the serializer.
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

			if (!(objectToSerialize is IList))
			{
				serializedResult = null;
				return false;
			}

			// Take the values from the original source values and process
			// them individually to data that is accepted by the serialization definition
			// and is accepted by the underlying type restrictions of the result collection.
			IList sourceValues = objectToSerialize as IList;
			IList processedValues = definition.CreateSequenceInstance(sourceValues.Count);
			SequenceCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(processedValues);
			for (int i = 0; i < sourceValues.Count; ++i)
			{
				object processedValue = Serializer.Serialize(sourceValues[i], definition);
				SerializationUtilities.InsertInSequence(processedValues, collectionInfo, i, processedValue);
			}

			serializedResult = processedValues;
			return true;
		}

		/// <summary>
		/// Attempts to deserialize the data to a new instance of the target type. Each element of the data is deserialized individually by the deserialized.
		/// </summary>
		/// <param name="targetType">The target type to deserialize the given data.</param>
		/// <param name="dataToDeserialize">The data deserialize and apply to the result.</param>
		/// <param name="deserializedResult">The result unto which the data is applied.</param>
		/// <returns>True if deserialization is compatible and accepted, false otherwise.</returns>
		public bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult)
		{
			// Check if the target implements the general IList interface, if not, we can just skip it altogether.
			if ((targetType == null) || !typeof(IList).IsAssignableFrom(targetType))
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

			if (!(dataToDeserialize is IList))
			{
				throw new SerializationException("The source value is expected to implement the {0} interface to process to target type {1}.", typeof(IList).Name, targetType.Name);
			}

			// Arrays are treated differently.
			IList targetCollection =
				targetType.IsArray ?
				Array.CreateInstance(targetType.GetElementType(), (dataToDeserialize as IList).Count) :
				Activator.CreateInstance(targetType, true) as IList;

			if (!Deserialize(targetCollection, dataToDeserialize))
			{
				throw new SerializationException("Unexpected failure to process source value of type {0} to target collection of type {1}.", dataToDeserialize.GetType().Name, targetType.Name);
			}

			deserializedResult = targetCollection;
			return true;
		}

		/// <summary>
		/// Attempts to deserialize the data onto an existing instance. Each element of the data is deserialized individually by the deserialized.
		/// </summary>
		/// <param name="deserializationTarget">The object on which the data should be applied.</param>
		/// <param name="dataToDeserialize">The data to deserialize.</param>
		/// <returns>True if the deserialization is compatible and accepted, false otherwise.</returns>
		public bool Deserialize(object deserializationTarget, object dataToDeserialize)
		{
			if ((deserializationTarget == null) || !(deserializationTarget is IList))
			{
				return false;
			}

			// If there is nothing to do...
			if (dataToDeserialize == null)
			{
				return true;
			}

			if (!(dataToDeserialize is IList))
			{
				throw new SerializationException("The source value is expected to implement the {0} interface to process to target instance of type {1}.", typeof(IList).Name, deserializationTarget.GetType().Name);
			}

			IList sourceValues = dataToDeserialize as IList;
			IList targetValues = deserializationTarget as IList;
			SequenceCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(targetValues);
			for (int i = 0; i < sourceValues.Count; ++i)
			{
				object processedValue = Serializer.Deserialize(collectionInfo.elementType, sourceValues[i], definition);
				processedValue = collectionInfo.PostProcessValue(processedValue);
				SerializationUtilities.InsertInSequence(targetValues, collectionInfo, i, processedValue);
			}

			return true;
		}
	}
}
