using System;

namespace ImpossibleOdds.Serialization.Processors
{
    public class VersionProcessor : ISerializationProcessor, IDeserializationProcessor
    {
        private int fieldCount = 3;
        
        public ISerializationDefinition Definition { get; }

        /// <summary>
        /// When the Version value is converted to a string, the field count determines how many components a version value should have.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The field count value is expected to be between 0 and 4.</exception>
        public int FieldCount
        {
            get => fieldCount;
            set
            {
                if (fieldCount is < 0 or > 4)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(FieldCount)} is expected to be between 0 and 4.");
                }

                fieldCount = value;
            }
        }
        
        public VersionProcessor(ISerializationDefinition definition)
        {
            definition.ThrowIfNull(nameof(definition));
            Definition = definition;
        }

        /// <inheritdoc />
        public virtual object Serialize(object objectToSerialize)
        {
            this.ThrowIfCantSerialize(objectToSerialize);
            return Definition.SupportedTypes.Contains(typeof(Version)) ? objectToSerialize : ((Version)objectToSerialize).ToString(fieldCount);
        }

        /// <inheritdoc />
        public virtual object Deserialize(Type targetType, object dataToDeserialize)
        {
            this.ThrowIfCantDeserialize(targetType, dataToDeserialize);

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
                (dataToDeserialize is Version or string);
        }
    }
}