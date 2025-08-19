using System;

namespace ImpossibleOdds.Serialization.Processors
{
	/// <summary>
	/// Simple (de)serialization processor that checks if the type and data are an exact match and can directly be applied.
	/// </summary>
	public class ExactMatchProcessor : ISerializationProcessor, IDeserializationProcessor
	{
		public ISerializationDefinition Definition { get; }

		public ExactMatchProcessor(ISerializationDefinition definition)
		{
			definition.ThrowIfNull(nameof(definition));
			Definition = definition;
		}

		/// <inheritdoc />
		public virtual object Serialize(object objectToSerialize)
		{
			this.ThrowIfCantSerialize(objectToSerialize);
			return objectToSerialize;
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			this.ThrowIfCantDeserialize(targetType, dataToDeserialize);
			return dataToDeserialize;
		}

		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			return
				(objectToSerialize != null) &&
				Definition.SupportedTypes.Contains(objectToSerialize.GetType());
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