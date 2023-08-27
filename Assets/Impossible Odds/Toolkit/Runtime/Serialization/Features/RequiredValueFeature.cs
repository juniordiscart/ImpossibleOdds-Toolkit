using System;

namespace ImpossibleOdds.Serialization
{
    /// <summary>
    /// Generic required value feature object that provides the type of the attributes to be placed atop
    /// data fields that are required to be present and/or not be null.
    /// </summary>
    /// <typeparam name="TRequiredValue">The type of the attribute to be defined on the required data fields.</typeparam>
    public class RequiredValueFeature<TRequiredValue> : IRequiredValueFeature
    where TRequiredValue : Attribute, IRequiredParameter
    {
        /// <inheritdoc />
        public Type RequiredValueAttributeType => typeof(TRequiredValue);
    }
}