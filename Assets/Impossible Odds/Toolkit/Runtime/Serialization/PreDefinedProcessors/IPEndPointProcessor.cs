using System;
using System.Net;

namespace ImpossibleOdds.Serialization.Processors
{
    public class IPEndPointProcessor : ISerializationProcessor, IDeserializationProcessor
    {
        private readonly ISerializationDefinition definition;

        public ISerializationDefinition Definition => definition;

        public IPEndPointProcessor(ISerializationDefinition definition)
        {
            definition.ThrowIfNull(nameof(definition));
            this.definition = definition;
        }
        
        public bool Serialize(object objectToSerialize, out object serializedResult)
        {
            if (objectToSerialize is not IPEndPoint ipAddress)
            {
                serializedResult = null;
                return false;
            }

            if (Definition.SupportedTypes.Contains(typeof(IPEndPoint)))
            {
                serializedResult = objectToSerialize;
                return true;
            }
            
            if (!Definition.SupportedTypes.Contains(typeof(string)))
            {
                throw new SerializationException($"The converted type of a {nameof(IPEndPoint)} type is not supported.");
            }
            
            serializedResult = ipAddress.ToString();
            return true;
        }
        
        public bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult)
        {
            targetType.ThrowIfNull(nameof(targetType));

            if ((dataToDeserialize == null) || !typeof(IPEndPoint).IsAssignableFrom(targetType))
            {
                deserializedResult = null;
                return false;
            }

            if (dataToDeserialize is IPEndPoint)
            {
                deserializedResult = dataToDeserialize;
                return true;
            }

            // At this point, a conversion is needed, but all types other than string will throw an exception.
            if (dataToDeserialize is not string ipEndPointString)
            {
                throw new SerializationException($"Only values of type {nameof(String)} can be used to convert to a {nameof(IPEndPoint)} value.");
            }

            try
            {
                string[] ipEndPointComponents = ipEndPointString.Split(':');
                IPAddress ipAddress = IPAddress.Parse(ipEndPointComponents[0]);
                int port = int.Parse(ipEndPointComponents[1]);

                deserializedResult = new IPEndPoint(ipAddress, port);
                return true;
            }
            catch (Exception e)
            {
                Log.Exception(e);
                throw new SerializationException($"Failed to deserialize a string representation of the type {nameof(IPEndPoint)}.");
            }
        }
    }
}

