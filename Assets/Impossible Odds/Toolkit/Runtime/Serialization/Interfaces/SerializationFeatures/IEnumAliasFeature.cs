using System;

namespace ImpossibleOdds.Serialization
{
    /// <summary>
    /// Enum alias serialization feature interface to define that an enum should be serialized
    /// as a string value instead of its underlying value.
    /// Additionally, individual enum values can be provided with an alias during serialization.
    /// </summary>
    public interface IEnumAliasFeature : ISerializationFeature
    {
        /// <summary>
        /// Type of the attribute to be defined atop of the enum definition.
        /// </summary>
        Type AsStringAttribute
        {
            get;
        }

        /// <summary>
        /// Type of the attribute to be defined on a single enum field that provides an alias for that enum value.
        /// </summary>
        Type AliasValueAttribute
        {
            get;
        }
    }
}