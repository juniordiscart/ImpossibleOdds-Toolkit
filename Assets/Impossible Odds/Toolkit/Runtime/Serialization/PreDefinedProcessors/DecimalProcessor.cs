using System;

namespace ImpossibleOdds.Serialization.Processors
{
    /// <summary>
    /// A (de)serialization processor specifically for the Decimal type.
    /// </summary>
    public class DecimalProcessor : ISerializationProcessor, IDeserializationProcessor
    {
        public ISerializationDefinition Definition { get; }

        public DecimalProcessor(ISerializationDefinition definition)
        {
            definition.ThrowIfNull(nameof(definition));
            Definition = definition;
        }

        /// <inheritdoc />
        public virtual object Serialize(object objectToSerialize)
        {
            if (!CanSerialize(objectToSerialize))
            {
                throw new SerializationException($"The provided data cannot be serialized by this processor of type {nameof(DecimalProcessor)}.");
            }

            // If the serialization definition supports the decimal-type, then just return already.
            // Otherwise, try to convert it to a string value.
            if (Definition.SupportedTypes.Contains(typeof(decimal)))
            {
                return objectToSerialize;
            }

            decimal value = (decimal)objectToSerialize;
            return value.ToString(Definition.FormatProvider);
        }

        /// <inheritdoc />
        public virtual object Deserialize(Type targetType, object dataToDeserialize)
        {
            if (!CanDeserialize(targetType, dataToDeserialize))
            {
                throw new SerializationException($"The provided data cannot be deserialized by this processor of type {nameof(DecimalProcessor)}.");
            }

            switch (dataToDeserialize)
            {
                case decimal _:
                    return dataToDeserialize;
                case string dStr:
                    try
                    {
                        return decimal.Parse(dStr);
                    }
                    catch (Exception e)
                    {
                        throw new SerializationException($"Failed to parse the string value to a value of type {nameof(Decimal)}.", e);
                    }
                case IConvertible convertible:
                    try
                    {
                        return convertible.ToDecimal(Definition.FormatProvider);
                    }
                    catch (Exception e)
                    {
                        throw new SerializationException($"Failed to convert the value to a value of type {nameof(Decimal)}.", e);
                    }
                default:
                    throw new SerializationException($"Failed to deserialize to a value of type {nameof(Decimal)}.");
            }
        }

        /// <inheritdoc />
        public virtual bool CanSerialize(object objectToSerialize)
        {
            return
                (objectToSerialize is decimal) &&
                (Definition.SupportedTypes.Contains(typeof(decimal)) || Definition.SupportedTypes.Contains(typeof(string)));
        }

        /// <inheritdoc />
        public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
        {
            targetType.ThrowIfNull(nameof(targetType));

            return
                (dataToDeserialize != null) &&
                typeof(decimal).IsAssignableFrom(targetType) &&
                ((dataToDeserialize is decimal) || (dataToDeserialize is string) || (dataToDeserialize is IConvertible));
        }
    }
}