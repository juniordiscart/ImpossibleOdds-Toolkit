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

		/// <inheritdoc />
		public virtual object Serialize(object objectToSerialize)
		{
			if (!CanSerialize(objectToSerialize))
			{
				throw new SerializationException("The provided data cannot be serialized by this processor of type {0}.", this.GetType().Name);
			}

			// If the serialization definition supports the decimal-type, then just return already.
			// Otherwise, try to convert it to a string value.
			if (Definition.SupportedTypes.Contains(typeof(decimal)))
			{
				return objectToSerialize;
			}
			else
			{
				decimal value = (decimal)objectToSerialize;
				return value.ToString();
			}
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			if (!CanDeserialize(targetType, dataToDeserialize))
			{
				throw new SerializationException("The provided data cannot be deserialized by this processor of type {0}.", this.GetType().Name);
			}

			if (dataToDeserialize is decimal)
			{
				return dataToDeserialize;
			}
			else
			{
				return decimal.Parse(dataToDeserialize as string);
			}
		}

		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			return
				(objectToSerialize != null) &&
				(objectToSerialize is decimal) &&
				(definition.SupportedTypes.Contains(typeof(decimal)) || definition.SupportedTypes.Contains(typeof(string)));
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			return
				(dataToDeserialize != null) &&
				typeof(decimal).IsAssignableFrom(targetType) &&
				((dataToDeserialize is decimal) || (dataToDeserialize is string));
		}
	}
}
