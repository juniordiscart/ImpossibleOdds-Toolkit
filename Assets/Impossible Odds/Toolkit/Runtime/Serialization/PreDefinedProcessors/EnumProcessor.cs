using System;
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

            if (SupportsEnumAlias && typeCache.PrefersStringBasedRepresentation(AliasFeature) && Definition.SupportedTypes.Contains(typeof(string)))
            {
                return typeCache.GetStringRepresentationFor(objectToSerialize as Enum, AliasFeature);
            }

            try
            {
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

            // If the data is a value type, convert it to the underlying type of the enum, and return that.
            if (SerializationUtilities.IsNumericIntegralType(dataToDeserialize.GetType()) || (dataToDeserialize is Enum))
            {
                Type underlyingEnumType = Enum.GetUnderlyingType(targetType);
                return Enum.ToObject(targetType, Convert.ChangeType(dataToDeserialize, underlyingEnumType));
            }

            // If at this point the data is not a string, then abort the deserialization.
            if (!(dataToDeserialize is string enumStr))
            {
                throw new SerializationException($"Could not process a value of type {dataToDeserialize.GetType()} to an instance of type {nameof(Enum)}.");
            }

            // If aliasing is supported, try to process individual values in the enum string
            if (SupportsEnumAlias)
            {
                return GetEnumTypeCache(targetType).GetEnumValueFor(enumStr, AliasFeature);
            }

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