using System;
using System.Globalization;

namespace ImpossibleOdds.Serialization.Processors
{
    /// <summary>
    /// A (de)serialization processor specifically to process DateTime values.
    /// </summary>
    public class DateTimeProcessor : ISerializationProcessor, IDeserializationProcessor
    {
        /// <summary>
        /// Serialize DateTime structs as a string, rather than a long integer.
        /// </summary>
        public bool PreferStringSerialization { get; set; } = true;

        /// <summary>
        /// The format used for DateTime string representation during serialization.
        /// </summary>
        public string DateTimeFormat { get; set; }

        public ISerializationDefinition Definition { get; }

        public DateTimeProcessor(ISerializationDefinition definition)
            : this(definition, string.Empty)
        {
        }

        public DateTimeProcessor(ISerializationDefinition definition, string dateTimeFormat)
        {
            definition.ThrowIfNull(nameof(definition));
            Definition = definition;
            DateTimeFormat = dateTimeFormat;
        }

        /// <inheritdoc />
        public virtual object Serialize(object objectToSerialize)
        {
            if (!CanSerialize(objectToSerialize))
            {
                throw new SerializationException($"The provided data cannot be serialized by this processor of type {nameof(DateTimeProcessor)}.");
            }

            // If the serialization definition supports the DateTime-type, then just return already.
            // Otherwise, try to convert it to a string value.
            if (Definition.SupportedTypes.Contains(typeof(DateTime)))
            {
                return objectToSerialize;
            }

            DateTime dtValue = (DateTime)objectToSerialize;

            if (PreferStringSerialization && Definition.SupportedTypes.Contains(typeof(string)))
            {
                return
                    string.IsNullOrWhiteSpace(DateTimeFormat) ? dtValue.ToString(Definition.FormatProvider) : dtValue.ToString(DateTimeFormat);
            }

            return dtValue.Ticks;
        }

        /// <inheritdoc />
        public virtual object Deserialize(Type targetType, object dataToDeserialize)
        {
            if (!CanDeserialize(targetType, dataToDeserialize))
            {
                throw new SerializationException($"The provided data cannot be deserialized by this processor of type {nameof(DateTimeProcessor)}.");
            }

            switch (dataToDeserialize)
            {
                case DateTime _:
                    return dataToDeserialize;
                case string dateTimeStr:
                    try
                    {
                        return
                            string.IsNullOrWhiteSpace(DateTimeFormat) ? DateTime.Parse(dateTimeStr, Definition.FormatProvider) : DateTime.ParseExact(dateTimeStr, DateTimeFormat, CultureInfo.InvariantCulture);
                    }
                    catch (Exception e)
                    {
                        throw new SerializationException($"Failed to parse the string value to a value of type {nameof(DateTime)}.", e);
                    }
                case IConvertible convertible:
                    try
                    {
                        long ticks = Convert.ToInt64(convertible);
                        return new DateTime(ticks);
                    }
                    catch (Exception e)
                    {
                        throw new SerializationException($"Failed to convert the value to a value of type {nameof(DateTime)}.", e);
                    }
                default:
                    throw new SerializationException($"Failed to deserialize to a value of type {nameof(DateTime)}.");
            }
        }

        /// <inheritdoc />
        public virtual bool CanSerialize(object objectToSerialize)
        {
            return
                (objectToSerialize is DateTime) &&
                (Definition.SupportedTypes.Contains(typeof(DateTime)) ||
                 Definition.SupportedTypes.Contains(typeof(long)) ||
                 (PreferStringSerialization && Definition.SupportedTypes.Contains(typeof(string))));
        }

        /// <inheritdoc />
        public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
        {
            targetType.ThrowIfNull(nameof(targetType));

            return
                (dataToDeserialize != null) &&
                typeof(DateTime).IsAssignableFrom(targetType) &&
                ((dataToDeserialize is DateTime) || (dataToDeserialize is string) || (dataToDeserialize is IConvertible));
        }
    }
}