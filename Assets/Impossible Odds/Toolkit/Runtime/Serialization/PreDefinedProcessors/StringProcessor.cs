using System;

namespace ImpossibleOdds.Serialization.Processors
{
    public class StringProcessor : IDeserializationProcessor
    {
        public ISerializationDefinition Definition { get; }

        public StringProcessor(ISerializationDefinition definition)
        {
            definition.ThrowIfNull(nameof(definition));
            Definition = definition;
        }

        /// <inheritdoc />
        public virtual object Deserialize(Type targetType, object dataToDeserialize)
        {
            this.ThrowIfCantDeserialize(targetType, dataToDeserialize);

            switch (dataToDeserialize)
            {
                case string str:
                    return str;
                case IConvertible convertible:
                    try
                    {
                        return convertible.ToString(Definition.FormatProvider);
                    }
                    catch (Exception e)
                    {
                        throw new SerializationException($"Failed to convert the value to a value of type {nameof(String)}.", e);
                    }
                default:
                    throw new SerializationException($"Failed to deserialize to a value of type {nameof(String)}.");
            }
        }

        /// <inheritdoc />
        public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
        {
            targetType.ThrowIfNull(nameof(targetType));

            return
                typeof(string).IsAssignableFrom(targetType) &&
                (dataToDeserialize is string or IConvertible);
        }
    }
}