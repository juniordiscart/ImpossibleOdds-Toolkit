using System;

namespace ImpossibleOdds.Serialization.Processors
{
	/// <summary>
	/// A (de)serialization processor specifically to process Guid values.
	/// </summary>
	public class GuidProcessor : ISerializationProcessor, IDeserializationProcessor
	{
		private string guidFormat = "D";
		
		public ISerializationDefinition Definition { get; }

		/// <summary>
		/// The format to serialize Guid values.
		/// </summary>
		public string Format
		{
			get => guidFormat;
			set
			{
				switch (value)
				{
					case null:
					case "D":
					case "N":
					case "B":
					case "P":
					case "X":
						guidFormat = value;
						break;
					default:
						throw new ArgumentOutOfRangeException($"{nameof(Format)} is expected to be one of the following values: null, \"\", 'D', 'N', 'B', 'P' or 'X'.");
				}
			}
		}

		public GuidProcessor(ISerializationDefinition definition)
		{
			definition.ThrowIfNull(nameof(definition));
			Definition = definition;
		}

		/// <inheritdoc />
		public virtual object Serialize(object objectToSerialize)
		{
			this.ThrowIfCantSerialize(objectToSerialize);
			return Definition.SupportedTypes.Contains(typeof(Guid)) ? objectToSerialize : ((Guid)objectToSerialize).ToString(Format);
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			this.ThrowIfCantDeserialize(targetType, dataToDeserialize);

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
				(objectToSerialize is Guid) &&
				(Definition.SupportedTypes.Contains(typeof(Guid)) || Definition.SupportedTypes.Contains(typeof(string)));
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));
			
			return
				(dataToDeserialize != null) &&
				typeof(Guid).IsAssignableFrom(targetType) &&
				(dataToDeserialize is Guid or string);
		}
	}
}