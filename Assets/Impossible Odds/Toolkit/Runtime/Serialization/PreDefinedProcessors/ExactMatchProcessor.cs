namespace ImpossibleOdds.Serialization.Processors
{
	using System;

	/// <summary>
	/// Simple (de)serialization processor that checks if the type and data are an exact match and can directly be applied.
	/// </summary>
	public class ExactMatchProcessor : ISerializationProcessor, IDeserializationProcessor
	{
		private readonly ISerializationDefinition definition = null;

		public ISerializationDefinition Definition
		{
			get => definition;
		}

		public ExactMatchProcessor(ISerializationDefinition definition)
		{
			this.definition = definition;
		}

		/// <inheritdoc />
		public virtual object Serialize(object objectToSerialize)
		{
			if (!CanSerialize(objectToSerialize))
			{
				throw new SerializationException("The provided data cannot be serialized by this processor of type {0}.", nameof(ExactMatchProcessor));
			}

			return objectToSerialize;
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			if (!CanDeserialize(targetType, dataToDeserialize))
			{
				throw new SerializationException("The provided data cannot be deserialized by this processor of type {0}.", nameof(ExactMatchProcessor));
			}

			return dataToDeserialize;
		}

		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			return
				(objectToSerialize != null) &&
				definition.SupportedTypes.Contains(objectToSerialize.GetType());
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			return
				((dataToDeserialize != null) && targetType.IsInstanceOfType(dataToDeserialize)) ||
				((dataToDeserialize == null) && SerializationUtilities.IsNullableType(targetType));
		}
	}
}