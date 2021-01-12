namespace ImpossibleOdds.Serialization.Processors
{
	using System;

	/// <summary>
	/// Simple (de)serialization processor that checks if the type and data are an exact match and can directly be applied.
	/// </summary>
	public class ExactMatchProcessor : ISerializationProcessor, IDeserializationProcessor
	{
		private ISerializationDefinition definition = null;

		public ISerializationDefinition Definition
		{
			get { return definition; }
		}

		public ExactMatchProcessor(ISerializationDefinition definition)
		{
			this.definition = definition;
		}

		/// <summary>
		/// Attempts to serialize the object to a directly supported type by the definition.
		/// </summary>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <param name="serializedResult">The serialized object.</param>
		/// <returns>True if the serialization is compatible and accepted, false otherwise.</returns>
		public bool Serialize(object objectToSerialize, out object serializedResult)
		{
			if ((objectToSerialize == null) || !definition.SupportedTypes.Contains(objectToSerialize.GetType()))
			{
				serializedResult = null;
				return false;
			}

			serializedResult = objectToSerialize;
			return true;
		}

		/// <summary>
		/// Attempts to directly assign if the target type is a match with the type of the deserialization data.
		/// </summary>
		/// <param name="targetType">The target type to deserialize the given data.</param>
		/// <param name="dataToDeserialize">The data deserialize and apply to the result.</param>
		/// <param name="deserializedResult">The result unto which the data is applied.</param>
		/// <returns>True if deserialization is compatible and accepted, false otherwise.</returns>
		public bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult)
		{
			if (targetType == null)
			{
				deserializedResult = null;
				return false;
			}

			// If the target type is nullable and we're dealing with a null value, then lets just call it quits.
			if ((dataToDeserialize == null) && !targetType.IsValueType)
			{
				deserializedResult = dataToDeserialize;
				return true;
			}

			if (!targetType.IsAssignableFrom(dataToDeserialize.GetType()))
			{
				deserializedResult = null;
				return false;
			}

			deserializedResult = dataToDeserialize;
			return true;
		}
	}
}
