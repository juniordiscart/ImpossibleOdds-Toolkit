using System;

namespace ImpossibleOdds.Serialization
{
    /// <summary>
    /// Base type resolution feature interface to define that a type supports serializing the necessary
    /// information to properly reconstruct it after deserialization.
    /// </summary>
    public interface ITypeResolutionFeature : ISerializationFeature
    {
        /// <summary>
        /// Type of the attribute to be applied atop of a class, struct or interface.
        /// </summary>
        Type TypeResolutionAttribute
        {
            get;
        }
    }
}