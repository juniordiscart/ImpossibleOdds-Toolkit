using System;

namespace ImpossibleOdds.Serialization.Processors
{
    /// <summary>
    /// A (de)serialization processor specifically to process Version values.
    /// </summary>
    public class VersionProcessor : ISerializationProcessor, IDeserializationProcessor
    {
        public ISerializationDefinition Definition { get; }

        public VersionProcessor(ISerializationDefinition definition)
        {
            definition.ThrowIfNull(nameof(definition));
            Definition = definition;
        }

        /// <inheritdoc />
        public virtual object Serialize(object objectToSerialize)
        {
            if (!CanSerialize(objectToSerialize))
            {
                throw new SerializationException($"The provided data cannot be serialized by this processor of type {nameof(VersionProcessor)}.");
            }

            // If the serialization definition supports the Version-type, then just return already.
            // Otherwise, try to convert it to a string value.
            if (Definition.SupportedTypes.Contains(typeof(Version)))
            {
                return objectToSerialize;
            }

            Version value = (Version)objectToSerialize;
            return value.ToString();
        }

        /// <inheritdoc />
        public virtual object Deserialize(Type targetType, object dataToDeserialize)
        {
            if (!CanDeserialize(targetType, dataToDeserialize))
            {
                throw new SerializationException($"The provided data cannot be deserialized by this processor of type {nameof(VersionProcessor)}.");
            }

            switch (dataToDeserialize)
            {
                case Version _:
                    return dataToDeserialize;
                case string vStr:
                    try
                    {
                        return Version.Parse(vStr);
                    }
                    catch (Exception e)
                    {
                        throw new SerializationException($"Failed to parse the string value to a value of type {nameof(Version)}.", e);
                    }
                default:
                    throw new SerializationException($"Failed to deserialize to a value of type {nameof(Version)}.");
            }
        }

        /// <inheritdoc />
        public virtual bool CanSerialize(object objectToSerialize)
        {
            return
                (objectToSerialize is Version) &&
                (Definition.SupportedTypes.Contains(typeof(Version)) || Definition.SupportedTypes.Contains(typeof(string)));
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