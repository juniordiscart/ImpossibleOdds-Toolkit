namespace ImpossibleOdds.Serialization.Processors
{
	using System;

	/// <summary>
	/// A deserialization processor that handles null values.
	/// If the target is a nullable type, null will be returned, otherwise, if the target
	/// is a value type, the default value for that type will be set on the target.
	/// </summary>
	public class NullValueProcessor : IDeserializationProcessor
	{
		private readonly ISerializationDefinition definition = null;

		public ISerializationDefinition Definition
		{
			get => definition;
		}

		public NullValueProcessor(ISerializationDefinition definition)
		{
			definition.ThrowIfNull(nameof(definition));
			this.definition = definition;
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			if (!CanDeserialize(targetType, dataToDeserialize))
			{
				throw new SerializationException("The provided data cannot be deserialized by this processor of type {0}.", nameof(NullValueProcessor));
			}

			return
				targetType.IsValueType ?
				Activator.CreateInstance(targetType) :
				null;
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));
			return (dataToDeserialize == null);
		}
	}
}