using System;

namespace ImpossibleOdds.Serialization
{
    /// <summary>
    /// Generic enum string and alias feature object that provides the types of the attributes to be placed
    /// atop of the enum definition, as well as over the individual enum values to define aliases.
    /// </summary>
    /// <typeparam name="TAsString">The type of the attribute to be defined on the enum definition.</typeparam>
    /// <typeparam name="TAlias">The type of the attribute to be defined on an enum value.</typeparam>
    public class EnumAliasFeature<TAsString, TAlias> : IEnumAliasFeature
    where TAsString : Attribute
    where TAlias : Attribute, IEnumAliasParameter
    {
        /// <inheritdoc />
        public Type AsStringAttributeType => typeof(TAsString);

        /// <inheritdoc />
        public Type AliasValueAttributeType => typeof(TAlias);
    }
}