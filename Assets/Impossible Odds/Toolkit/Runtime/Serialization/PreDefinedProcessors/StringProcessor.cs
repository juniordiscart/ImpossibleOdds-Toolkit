namespace ImpossibleOdds.Serialization.Processors
{
	using System;

	/// <summary>
	/// A simple deserialization-only processor to process a value to a string.
	/// </summary>
	public class StringProcessor : IDeserializationProcessor
	{
		private readonly ISerializationDefinition definition = null;

		public ISerializationDefinition Definition
		{
			get => definition;
		}

		public StringProcessor(ISerializationDefinition definition)
		{
			this.definition = definition;
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			if (!CanDeserialize(targetType, dataToDeserialize))
			{
				throw new SerializationException("The provided data cannot be deserialized by this processor of type {0}.", nameof(StringProcessor));
			}

			return
				dataToDeserialize is string ?
					dataToDeserialize :
					Convert.ChangeType(dataToDeserialize, targetType, definition.FormatProvider);
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			return
				typeof(string).IsAssignableFrom(targetType) &&
				((dataToDeserialize is string) || (dataToDeserialize is IConvertible));
		}
	}
}