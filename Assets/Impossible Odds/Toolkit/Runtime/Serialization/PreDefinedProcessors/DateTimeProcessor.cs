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
        public string Format { get; set; } = string.Empty;

        public ISerializationDefinition Definition { get; }

        public DateTimeProcessor(ISerializationDefinition definition)
        {
            definition.ThrowIfNull(nameof(definition));
            Definition = definition;
        }

        /// <inheritdoc />
        public virtual object Serialize(object objectToSerialize)
        {
            this.ThrowIfCantSerialize(objectToSerialize);

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
                    string.IsNullOrWhiteSpace(Format) ? dtValue.ToString(Definition.FormatProvider) : dtValue.ToString(Format);
            }

            return dtValue.Ticks;
        }

        /// <inheritdoc />
        public virtual object Deserialize(Type targetType, object dataToDeserialize)
        {
            this.ThrowIfCantDeserialize(targetType, dataToDeserialize);

            switch (dataToDeserialize)
            {
                case DateTime _:
                    return dataToDeserialize;
                case string dateTimeStr:
                    try
                    {
                        return
                            string.IsNullOrWhiteSpace(Format) ? DateTime.Parse(dateTimeStr, Definition.FormatProvider) : DateTime.ParseExact(dateTimeStr, Format, CultureInfo.InvariantCulture);
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
                (dataToDeserialize is DateTime or string or IConvertible);
        }
    }
}