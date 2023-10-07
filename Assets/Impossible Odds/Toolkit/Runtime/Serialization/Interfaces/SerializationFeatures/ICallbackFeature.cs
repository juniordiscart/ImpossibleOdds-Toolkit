using System;

namespace ImpossibleOdds.Serialization
{
    /// <summary>
    /// Callback serialization feature interface to define that an object can request to receive
    /// callbacks whenever a serialization action begins or ends on its data.
    /// </summary>
    public interface ICallbackFeature : ISerializationFeature
    {
        /// <summary>
        /// Type of the attribute to be defined on methods that should be called when serialization begins on the object.
        /// </summary>
        Type OnSerializationAttribute
        {
            get;
        }

        /// <summary>
        /// Type of the attribute to be defined on methods that should be called when serialization has ended on the object.
        /// </summary>
        Type OnSerializedAttribute
        {
            get;
        }

        /// <summary>
        /// Type of the attribute to be defined on methods that should be called when deserialization begins on the object.
        /// </summary>
        Type OnDeserializationAttribute
        {
            get;
        }

        /// <summary>
        /// Type of the attribute to be define don methods that should be called when deserialization has ended on the object.
        /// </summary>
        Type OnDeserializedAttribute
        {
            get;
        }
    }
}