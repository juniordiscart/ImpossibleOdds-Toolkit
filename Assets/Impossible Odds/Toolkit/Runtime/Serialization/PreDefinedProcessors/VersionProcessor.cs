namespace ImpossibleOdds.Serialization.Processors
{
	using System;

	/// <summary>
	/// A (de)serialization processor specifically to process Version values.
	/// </summary>
	public class VersionProcessor : ISerializationProcessor, IDeserializationProcessor
	{
		private readonly ISerializationDefinition definition = null;

		public ISerializationDefinition Definition
		{
			get => definition;
		}

		public VersionProcessor(ISerializationDefinition definition)
		{
			definition.ThrowIfNull(nameof(definition));
			this.definition = definition;
		}

		/// <inheritdoc />
		public virtual object Serialize(object objectToSerialize)
		{
			if (!CanSerialize(objectToSerialize))
			{
				throw new SerializationException("The provided data cannot be serialized by this processor of type {0}.", nameof(VersionProcessor));
			}

			// If the serialization definition supports the Version-type, then just return already.
			// Otherwise, try to convert it to a string value.
			if (Definition.SupportedTypes.Contains(typeof(Version)))
			{
				return objectToSerialize;
			}
			else
			{
				Version value = (Version)objectToSerialize;
				return value.ToString();
			}
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			if (!CanDeserialize(targetType, dataToDeserialize))
			{
				throw new SerializationException("The provided data cannot be deserialized by this processor of type {0}.", nameof(VersionProcessor));
			}

			return dataToDeserialize is Version ? dataToDeserialize : Version.Parse(dataToDeserialize as string);
		}

		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			return
				(objectToSerialize != null) &&
				(objectToSerialize is Version) &&
				(definition.SupportedTypes.Contains(typeof(Version)) || definition.SupportedTypes.Contains(typeof(string)));
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			return
				(dataToDeserialize != null) &&
				typeof(Version).IsAssignableFrom(targetType) &&
				((dataToDeserialize is Version) || (dataToDeserialize is string));
		}
	}
}