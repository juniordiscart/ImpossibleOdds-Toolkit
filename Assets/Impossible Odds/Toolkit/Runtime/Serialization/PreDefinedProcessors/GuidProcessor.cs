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

		/// <inheritdoc />
		public virtual object Serialize(object objectToSerialize)
		{
			if (!CanSerialize(objectToSerialize))
			{
				throw new SerializationException("The provided data cannot be serialized by this processor of type {0}.", this.GetType().Name);
			}

			// If the serialization definition supports the Guid-type, then just return already.
			// Otherwise, try to convert it to a string value.
			if (Definition.SupportedTypes.Contains(typeof(Guid)))
			{
				return objectToSerialize;
			}
			else
			{
				return ((Guid)objectToSerialize).ToString(Format);
			}
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			if (!CanDeserialize(targetType, dataToDeserialize))
			{
				throw new SerializationException("The provided data cannot be deserialized by this processor of type {0}.", this.GetType().Name);
			}

			if (dataToDeserialize is Guid)
			{
				return dataToDeserialize;
			}
			else
			{
				return Guid.Parse(dataToDeserialize as string);
			}
		}

		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			return
				(objectToSerialize != null) &&
				(objectToSerialize is Guid) &&
				(definition.SupportedTypes.Contains(typeof(Guid)) || definition.SupportedTypes.Contains(typeof(string)));
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));
			
			return
				(dataToDeserialize != null) &&
				typeof(Guid).IsAssignableFrom(targetType) &&
				((dataToDeserialize is Guid) || (dataToDeserialize is string));
		}
	}
}
