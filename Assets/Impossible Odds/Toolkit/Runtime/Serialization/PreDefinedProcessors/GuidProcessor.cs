namespace ImpossibleOdds.Serialization.Processors
{
	using System;

	/// <summary>
	/// A (de)serialization processor specifically to process Guid values.
	/// </summary>
	public class GuidProcessor : ISerializationProcessor, IDeserializationProcessor
	{
		private string format = string.Empty;
		private readonly ISerializationDefinition definition = null;

		public ISerializationDefinition Definition
		{
			get => definition;
		}

		/// <summary>
		/// The format to serialize Guid values.
		/// Default format is 'D'.
		/// </summary>
		public string Format
		{
			get => format;
			set => format = value;
		}

		public GuidProcessor(ISerializationDefinition definition)
		: this(definition, "D")
		{ }

		public GuidProcessor(ISerializationDefinition definition, string format)
		{
			definition.ThrowIfNull(nameof(definition));
			this.definition = definition;
			Format = format;
		}

		/// <summary>
		/// Attempt to serialize the object as a Guid value.
		/// </summary>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <param name="serializedResult">The serialized object.</param>
		/// <returns>True if the serialization is compatible and accepted, false otherwise.</returns>
		public bool Serialize(object objectToSerialize, out object serializedResult)
		{
			if ((objectToSerialize == null) || !typeof(Guid).IsAssignableFrom(objectToSerialize.GetType()))
			{
				serializedResult = null;
				return false;
			}

			if (Definition.SupportedTypes.Contains(typeof(Guid)))
			{
				serializedResult = objectToSerialize;
				return true;
			}

			if (!Definition.SupportedTypes.Contains(typeof(string)))
			{
				throw new SerializationException("The converted type of a {0} type is not supported.", typeof(Guid).Name);
			}

			Guid guid = (Guid)objectToSerialize;
			serializedResult = guid.ToString(Format);
			return true;
		}

		/// <summary>
		/// Attempt to deserialize the object to a Guid value.
		/// </summary>
		/// <param name="targetType">The target type to deserialize the given data.</param>
		/// <param name="dataToDeserialize">The data deserialize and apply to the result.</param>
		/// <param name="deserializedResult">The result unto which the data is applied.</param>
		/// <returns>True if deserialization is compatible and accepted, false otherwise.</returns>
		public bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult)
		{
			targetType.ThrowIfNull(nameof(targetType));

			if ((dataToDeserialize == null) || !typeof(Guid).IsAssignableFrom(targetType))
			{
				deserializedResult = null;
				return false;
			}

			if (dataToDeserialize is Guid)
			{
				deserializedResult = dataToDeserialize;
				return true;
			}

			// At this point, a conversion is needed, but all types other than string will throw an exception.
			if (!(dataToDeserialize is string))
			{
				throw new SerializationException("Only values of type {0} can be used to convert to a {1} value.", typeof(string).Name, typeof(Guid).Name);
			}

			deserializedResult = Guid.Parse(dataToDeserialize as string);
			return true;
		}
	}
}
