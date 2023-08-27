using System;

namespace ImpossibleOdds.Serialization
{
    /// <summary>
    /// Required value feature interface to define that certain values in the data are required to be present and/or not be null.
    /// </summary>
    public interface IRequiredValueFeature : ISerializationFeature
    {
        /// <summary>
        /// Type of the attribute to be defined on data members to state that these are values that are required to be present in the data.
        /// </summary>
        Type RequiredValueAttributeType
        {
            get;
        }
    }
}