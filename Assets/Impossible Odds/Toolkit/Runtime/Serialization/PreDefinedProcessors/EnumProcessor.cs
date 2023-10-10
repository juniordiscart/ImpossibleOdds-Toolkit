using System;
using System.Collections;
using System.Collections.Concurrent;
using ImpossibleOdds.Serialization.Caching;

namespace ImpossibleOdds.Serialization.Processors
{
	/// <summary>
	/// A (de)serialization processor for enum values. This includes potential alias values for the enum values.
	/// </summary>
	public class EnumProcessor : ISerializationProcessor, IDeserializationProcessor
	{
		private static readonly ConcurrentDictionary<Type, EnumSerializationReflectionMap> EnumTypeCache = new ConcurrentDictionary<Type, EnumSerializationReflectionMap>();

		public bool SupportsEnumAlias => AliasFeature != null;

		public ISerializationDefinition Definition { get; }

		/// <summary>
		/// The enum alias feature to determine string-based representations of enum definitions.
		/// </summary>
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
				throw new SerializationException($"The provided data cannot be serialized by this processor of type {nameof(EnumProcessor)}.");
			}

			Type sourceType = objectToSerialize.GetType();
			EnumSerializationReflectionMap typeCache = GetEnumTypeCache(sourceType);

			try
			{
				if (SupportsEnumAlias && typeCache.PrefersStringBasedRepresentation(AliasFeature) && Definition.SupportedTypes.Contains(typeof(string)))
				{
					return typeCache.GetStringRepresentationFor(objectToSerialize as Enum, AliasFeature);
				}

				// Exceptions may be triggered if the underlying value of the enum is not supported.
				return Serializer.Serialize(Convert.ChangeType(objectToSerialize, Enum.GetUnderlyingType(sourceType)), Definition);
			}
			catch (Exception e)
			{
				throw new SerializationException($"Failed to serialize an enum value of type {sourceType.Name} to its underlying type of {Enum.GetUnderlyingType(sourceType).Name}", e);
			}
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			if (!CanDeserialize(targetType, dataToDeserialize))
			{
				throw new SerializationException($"The provided data cannot be deserialized by this processor of type {nameof(EnumProcessor)}.");
			}

			// If the data is an integral numeric value, try to convert it to a valid value of the enum.
			if (SerializationUtilities.IsNumericIntegralType(dataToDeserialize.GetType()) || (dataToDeserialize is Enum))
			{
				return Enum.ToObject(targetType, dataToDeserialize);
			}

			string strValue = (string)dataToDeserialize;

			try
			{
				return
					SupportsEnumAlias ?
						GetEnumTypeCache(targetType).GetEnumValueFor(strValue, AliasFeature) :
						Enum.Parse(targetType, strValue);
			}
			catch (Exception e)
			{
				throw new SerializationException($"Failed to deserialize the value '{strValue}' to a known value of Enum {targetType.Name}", e);
			}
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
				(SerializationUtilities.IsNumericIntegralType(dataToDeserialize.GetType()) || (dataToDeserialize is string) || (dataToDeserialize is Enum));
		}

		private EnumSerializationReflectionMap GetEnumTypeCache(Type enumType)
		{
			enumType.ThrowIfNull(nameof(enumType));

			if (!enumType.IsEnum)
			{
				throw new ArgumentException($"The type {enumType.Name} is not an enum.");
			}

			return EnumTypeCache.GetOrAdd(enumType, (type) => new EnumSerializationReflectionMap(enumType));
		}
	}
}