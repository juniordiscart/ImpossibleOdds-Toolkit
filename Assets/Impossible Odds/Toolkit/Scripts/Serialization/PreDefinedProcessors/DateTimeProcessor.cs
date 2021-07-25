namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Globalization;

	/// <summary>
	/// A (de)serialization processor specifically to process DateTime values.
	/// </summary>
	public class DateTimeProcessor : ISerializationProcessor, IDeserializationProcessor
	{
		private string dateTimeFormat = string.Empty;
		private ISerializationDefinition definition = null;

		public ISerializationDefinition Definition
		{
			get { return definition; }
		}

		public DateTimeProcessor(ISerializationDefinition definition)
		{
			this.definition = definition;
		}

		public DateTimeProcessor(ISerializationDefinition definition, string dateTimeFormat)
		{
			this.definition = definition;
			this.dateTimeFormat = dateTimeFormat;
		}

		/// <summary>
		/// Attempt to serialize the object as a DateTime value.
		/// </summary>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <param name="serializedResult">The serialized object.</param>
		/// <returns>True if the serialization is compatible and accepted, false otherwise.</returns>
		public bool Serialize(object objectToSerialize, out object serializedResult)
		{
			if ((objectToSerialize == null) || !typeof(DateTime).IsAssignableFrom(objectToSerialize.GetType()))
			{
				serializedResult = null;
				return false;
			}

			if (definition.SupportedTypes.Contains(typeof(DateTime)))
			{
				serializedResult = objectToSerialize;
				return true;
			}

			DateTime dtValue = (DateTime)objectToSerialize;
			string strValue = string.IsNullOrWhiteSpace(dateTimeFormat) ? dtValue.ToString(definition.FormatProvider) : dtValue.ToString(dateTimeFormat);

			if (!definition.SupportedTypes.Contains(strValue.GetType()))
			{
				throw new SerializationException("The converted type of a {0} type is not supported.", typeof(DateTime).Name);
			}

			serializedResult = strValue;
			return true;
		}

		/// <summary>
		/// Attempt to deserialize the object to a DateTime value.
		/// </summary>
		/// <param name="targetType">The target type to deserialize the given data.</param>
		/// <param name="dataToDeserialize">The data deserialize and apply to the result.</param>
		/// <param name="deserializedResult">The result unto which the data is applied.</param>
		/// <returns>True if deserialization is compatible and accepted, false otherwise.</returns>
		public bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult)
		{
			if ((targetType == null) || (dataToDeserialize == null) || !typeof(DateTime).IsAssignableFrom(targetType))
			{
				deserializedResult = null;
				return false;
			}

			if (dataToDeserialize is DateTime)
			{
				deserializedResult = dataToDeserialize;
				return true;
			}

			// At this point, a conversion is needed, but all types other than string will throw an exception.
			if (!(dataToDeserialize is string))
			{
				throw new SerializationException("Only values of type {0} can be used to convert to a {1} value.", typeof(string).Name, typeof(DateTime).Name);
			}

			deserializedResult = string.IsNullOrWhiteSpace(dateTimeFormat) ? DateTime.Parse(dataToDeserialize as string, definition.FormatProvider) : DateTime.ParseExact(dataToDeserialize as string, dateTimeFormat, CultureInfo.InvariantCulture);
			return true;
		}
	}
}
