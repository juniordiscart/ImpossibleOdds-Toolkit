using System;

namespace ImpossibleOdds.Serialization.Processors
{
	/// <summary>
	/// A (de)serialization processor specifically to process Guid values.
	/// </summary>
	public class GuidProcessor : ISerializationProcessor, IDeserializationProcessor
	{
		public ISerializationDefinition Definition { get; }

		/// <summary>
		/// The format to serialize Guid values.
		/// Default format is 'D'.
		/// </summary>
		public string Format { get; set; }

		public GuidProcessor(ISerializationDefinition definition, string format = "D")
		{
			definition.ThrowIfNull(nameof(definition));
			Definition = definition;
			Format = format;
		}

		/// <inheritdoc />
		public virtual object Serialize(object objectToSerialize)
		{
			if (!CanSerialize(objectToSerialize))
			{
				throw new SerializationException($"The provided data cannot be serialized by this processor of type {nameof(GuidProcessor)}.");
			}

			// If the serialization definition supports the Guid-type, then just return already.
			// Otherwise, try to convert it to a string value.
			return
				Definition.SupportedTypes.Contains(typeof(Guid)) ?
					objectToSerialize :
					((Guid)objectToSerialize).ToString(Format);
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			if (!CanDeserialize(targetType, dataToDeserialize))
			{
				throw new SerializationException($"The provided data cannot be deserialized by this processor of type {nameof(GuidProcessor)}.");
			}

			switch (dataToDeserialize)
			{
				case Guid _:
					return dataToDeserialize;
				case string guidStr:
					try
					{
						return Guid.Parse(guidStr);
					}
					catch (Exception e)
					{
						throw new SerializationException($"Failed to parse the string value to a value of type {nameof(Guid)}.", e);
					}
				default:
					throw new SerializationException($"Failed to deserialize to a value of type {nameof(Decimal)}.");
			}
		}

		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			return
				objectToSerialize is Guid &&
				(Definition.SupportedTypes.Contains(typeof(Guid)) || Definition.SupportedTypes.Contains(typeof(string)));
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