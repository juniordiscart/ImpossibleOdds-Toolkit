using System;
using System.Net;

namespace ImpossibleOdds.Serialization.Processors
{
    public class IPAddressProcessor : ISerializationProcessor, IDeserializationProcessor
    {
        private readonly ISerializationDefinition definition;

        public ISerializationDefinition Definition => definition;

        public IPAddressProcessor(ISerializationDefinition definition)
        {
            definition.ThrowIfNull(nameof(definition));
            this.definition = definition;
        }
        
        public bool Serialize(object objectToSerialize, out object serializedResult)
        {
            if (objectToSerialize is not IPAddress ipAddress)
            {
                serializedResult = null;
                return false;
            }

            if (Definition.SupportedTypes.Contains(typeof(IPAddress)))
            {
                serializedResult = objectToSerialize;
                return true;
            }
            
            if (!Definition.SupportedTypes.Contains(typeof(string)))
            {
                throw new SerializationException($"The converted type of a {nameof(IPAddress)} type is not supported.");
            }
            
            serializedResult = ipAddress.ToString();
            return true;
        }
        
        public bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult)
        {
            targetType.ThrowIfNull(nameof(targetType));

            if ((dataToDeserialize == null) || !typeof(IPAddress).IsAssignableFrom(targetType))
            {
                deserializedResult = null;
                return false;
            }

            if (dataToDeserialize is IPAddress)
            {
                deserializedResult = dataToDeserialize;
                return true;
            }

            // At this point, a conversion is needed, but all types other than string will throw an exception.
            if (dataToDeserialize is not string ipAddressString)
            {
                throw new SerializationException($"Only values of type {nameof(String)} can be used to convert to a {nameof(IPAddress)} value.");
            }

            if (!IPAddress.TryParse(ipAddressString, out IPAddress ipAddress))
            {
                deserializedResult = ipAddress;
                return true;
            }

            throw new SerializationException($"Failed to deserialize a string representation of the type {nameof(IPAddress)}.");
        }
    }
}

