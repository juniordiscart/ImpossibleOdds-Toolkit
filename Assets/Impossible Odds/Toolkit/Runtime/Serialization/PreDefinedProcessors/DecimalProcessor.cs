namespace ImpossibleOdds.Serialization.Processors
{
	using System;

	/// <summary>
	/// A (de)serialization processor specifically for the Decimal type.
	/// </summary>
	public class DecimalProcessor : ISerializationProcessor, IDeserializationProcessor
	{
		private readonly ISerializationDefinition definition = null;

		public ISerializationDefinition Definition
		{
			get => definition;
		}

		public DecimalProcessor(ISerializationDefinition definition)
		{
			this.definition = definition;
		}

		/// <summary>
		/// Attempts to serialize the object as a Decimal to a supported type as defined by serialization definition.
		/// </summary>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <param name="serializedResult">The serialized object.</param>
		/// <returns>True if the serialization is compatible and accepted, false otherwise.</returns>
		public bool Serialize(object objectToSerialize, out object serializedResult)
		{
			if ((objectToSerialize == null) || !typeof(decimal).IsAssignableFrom(objectToSerialize.GetType()))
			{
				serializedResult = null;
				return false;
			}

			if (definition.SupportedTypes.Contains(typeof(decimal)))
			{
				serializedResult = objectToSerialize;
				return true;
			}

			string strValue = ((decimal)objectToSerialize).ToString(definition.FormatProvider);
			if (!definition.SupportedTypes.Contains(strValue.GetType()))
			{
				throw new SerializationException("The converted type of a {0} type is not supported.", typeof(decimal).Name);
			}

			serializedResult = strValue;
			return true;
		}

		/// <summary>
		/// Attempts to deserialize the object to an instance of type Decimal, if the target type is a Decimal.
		/// </summary>
		/// <param name="targetType">The target type to deserialize the given data.</param>
		/// <param name="dataToDeserialize">The data deserialize and apply to the result.</param>
		/// <param name="deserializedResult">The result unto which the data is applied.</param>
		/// <returns>True if deserialization is compatible and accepted, false otherwise.</returns>
		public bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult)
		{
			targetType.ThrowIfNull(nameof(targetType));

			if ((dataToDeserialize == null) || !typeof(decimal).IsAssignableFrom(targetType))
			{
				deserializedResult = null;
				return false;
			}

			if (dataToDeserialize is decimal)
			{
				deserializedResult = dataToDeserialize;
				return true;
			}

			// At this point, a conversion is needed, but all types other than string will throw an exception.
			if (!(dataToDeserialize is string))
			{
				throw new SerializationException("Only values of type {0} can be used to convert to a {1} value.", typeof(string).Name, typeof(DateTime).Name);
			}

			deserializedResult = decimal.Parse(dataToDeserialize as string);
			return true;
		}

	}
}
