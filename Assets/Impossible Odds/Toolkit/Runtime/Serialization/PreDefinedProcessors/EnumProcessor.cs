namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections.Concurrent;
	using ImpossibleOdds.Serialization.Caching;

	/// <summary>
	/// A (de)serialization processor for enum values. This includes potential alias values for the enum values.
	/// </summary>
	public class EnumProcessor : ISerializationProcessor, IDeserializationProcessor
	{
		private static ConcurrentDictionary<Type, EnumSerializationReflectionMap> enumTypeCache = new ConcurrentDictionary<Type, EnumSerializationReflectionMap>();

		public bool SupportsEnumAlias => AliasFeature != null;

		public ISerializationDefinition Definition { get; }

		public IEnumAliasFeature AliasFeature { get; set; }

		public EnumProcessor(ISerializationDefinition definition)
		{
			Definition = definition;
		}

		/// <inheritdoc />
		public virtual object Serialize(object objectToSerialize)
		{
			if (!CanSerialize(objectToSerialize))
			{
				throw new SerializationException("The provided data cannot be serialized by this processor of type {0}.", nameof(EnumProcessor));
			}

			Type sourceType = objectToSerialize.GetType();
			Enum enumValue = objectToSerialize as Enum;
			EnumSerializationReflectionMap typeCache = GetEnumTypeCache(sourceType);

			// If the definition supports enum aliases and is preferred to be sent serialized as string.
			// Else, the enum should be serialized using its underlying type.
			if (SupportsEnumAlias && typeCache.PrefersStringBasedRepresentation(AliasFeature))
			{
				return typeCache.GetStringRepresentationFor(enumValue, AliasFeature);
			}
			else
			{
				return Serializer.Serialize(Convert.ChangeType(objectToSerialize, Enum.GetUnderlyingType(sourceType)), Definition);
			}
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			if (!CanDeserialize(targetType, dataToDeserialize))
			{
				throw new SerializationException("The provided data cannot be deserialized by this processor of type {0}.", nameof(EnumProcessor));
			}

			if (dataToDeserialize.GetType().IsValueType)
			{
				Type underlyingEnumType = Enum.GetUnderlyingType(targetType);
				return Enum.ToObject(targetType, Convert.ChangeType(dataToDeserialize, underlyingEnumType));
			}

			if (dataToDeserialize is string enumStr)
			{
				// If aliasing is supported, try to process individual values in the enum string
				if (SupportsEnumAlias)
				{
					return GetEnumTypeCache(targetType).GetEnumValueFor(enumStr, AliasFeature);
				}
				else
				{
					// If the string represents a numeric value, try to match to an internal value of the enum.
					// This may throw an exception if the value is not a numeric one. We let that fail silently and try a different approach.
					try
					{
						return Enum.ToObject(targetType, Convert.ChangeType(enumStr, Enum.GetUnderlyingType(targetType)));
					}
					catch
					{
						// Final resort, just try to parse it.
						return Enum.Parse(targetType, enumStr);
					}
				}
			}

			throw new SerializationException("Could not process a value of type {0} to an instance of type {1}.", dataToDeserialize.GetType(), nameof(Enum));
		}

		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			return
				(objectToSerialize != null) &&
				objectToSerialize.GetType().IsEnum;
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			return
				targetType.IsEnum &&
				(dataToDeserialize != null) &&
				(dataToDeserialize.GetType().IsValueType || (dataToDeserialize is string));
		}

		private EnumSerializationReflectionMap GetEnumTypeCache(Type enumType)
		{
			enumType.ThrowIfNull(nameof(enumType));

			if (!enumType.IsEnum)
			{
				throw new ArgumentException($"The type {enumType.Name} is not an enum.");
			}

			return enumTypeCache.GetOrAdd(enumType, (type) => new EnumSerializationReflectionMap(enumType));
		}
	}
}