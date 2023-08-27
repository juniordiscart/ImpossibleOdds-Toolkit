using System;

namespace ImpossibleOdds.Serialization
{
    /// <summary>
    /// Generic serialization callback feature settings object that provides the types of the attributes to be placed
    /// atop of the methods that should function as callbacks.
    /// </summary>
    /// <typeparam name="TSerialization">The type of the attribute that defines a callback whenever an object is about to be serialized.</typeparam>
    /// <typeparam name="TSerialized">The type of the attribute that defines a callback whenever an object is done being serialized.</typeparam>
    /// <typeparam name="TDeserialization">The type of the attribute that defines a callback whenever an object is about to be deserialized.</typeparam>
    /// <typeparam name="TDeserialized">The type of the attribute that defines a callback whenever an object is done being deserialized.</typeparam>
    public class CallBackFeature<TSerialization, TSerialized, TDeserialization, TDeserialized> : ICallbackFeature
    where TSerialization : Attribute
    where TSerialized : Attribute
    where TDeserialization : Attribute
    where TDeserialized : Attribute
    {
        /// <inheritdoc />
        public Type OnSerializationAttributeType => typeof(TSerialization);

        /// <inheritdoc />
        public Type OnSerializedAttributeType => typeof(TSerialized);

        /// <inheritdoc />
        public Type OnDeserializationAttributeType => typeof(TDeserialization);

        /// <inheritdoc />
        public Type OnDeserializedAttributeType => typeof(TDeserialized);
    }
}