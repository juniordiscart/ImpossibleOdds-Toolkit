namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Globalization;

	/// <summary>
	/// A (de)serialization processor specifically to process DateTime values.
	/// </summary>
	public class DateTimeProcessor : ISerializationProcessor, IDeserializationProcessor
	{
		private readonly string dateTimeFormat = string.Empty;
		private readonly ISerializationDefinition definition = null;

		public ISerializationDefinition Definition
		{
			get => definition;
		}

		public DateTimeProcessor(ISerializationDefinition definition)
		: this(definition, string.Empty)
		{ }

		public DateTimeProcessor(ISerializationDefinition definition, string dateTimeFormat)
		{
			definition.ThrowIfNull(nameof(definition));
			this.definition = definition;
			this.dateTimeFormat = dateTimeFormat;
		}

		/// <inheritdoc />
		public virtual object Serialize(object objectToSerialize)
		{
			if (!CanSerialize(objectToSerialize))
			{
				throw new SerializationException("The provided data cannot be serialized by this processor of type {0}.", this.GetType().Name);
			}

			// If the serialization definition supports the DateTime-type, then just return already.
			// Otherwise, try to convert it to a string value.
			if (Definition.SupportedTypes.Contains(typeof(DateTime)))
			{
				return objectToSerialize;
			}
			else
			{
				DateTime dtValue = (DateTime)objectToSerialize;
				return
					string.IsNullOrWhiteSpace(dateTimeFormat) ?
					dtValue.ToString(Definition.FormatProvider) :
					dtValue.ToString(dateTimeFormat);
			}
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			if (!CanDeserialize(targetType, dataToDeserialize))
			{
				throw new SerializationException("The provided data cannot be deserialized by this processor of type {0}.", this.GetType().Name);
			}

			if (dataToDeserialize is DateTime)
			{
				return dataToDeserialize;
			}
			else
			{
				string dateTimeStr = dataToDeserialize as string;
				return
					string.IsNullOrWhiteSpace(dateTimeFormat) ?
					DateTime.Parse(dateTimeStr, Definition.FormatProvider) :
					DateTime.ParseExact(dateTimeStr, dateTimeFormat, CultureInfo.InvariantCulture);
			}
		}

		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			return
				(objectToSerialize != null) &&
				(objectToSerialize is DateTime) &&
				(definition.SupportedTypes.Contains(typeof(DateTime)) || definition.SupportedTypes.Contains(typeof(string)));
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			return
				(dataToDeserialize != null) &&
				typeof(DateTime).IsAssignableFrom(targetType) &&
				((dataToDeserialize is DateTime) || (dataToDeserialize is string));
		}
	}
}
