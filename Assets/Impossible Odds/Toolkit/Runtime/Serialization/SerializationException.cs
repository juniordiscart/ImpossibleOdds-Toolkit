using System;

namespace ImpossibleOdds.Serialization
{
    public sealed class SerializationException : ImpossibleOddsException
    {
        public SerializationException()
        {
        }

        public SerializationException(string errMsg)
            : base(errMsg)
        {
        }

        public SerializationException(string errMsg, Exception innerException)
            : base(errMsg, innerException)
        {
        }
    }
}