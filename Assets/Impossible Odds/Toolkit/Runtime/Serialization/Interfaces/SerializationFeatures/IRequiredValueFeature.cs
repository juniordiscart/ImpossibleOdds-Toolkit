using System;
using ImpossibleOdds.Serialization.Caching;

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
        Type RequiredValueAttribute
        {
            get;
        }

        /// <summary>
        /// Is the member required by the target type?
        /// </summary>
        /// <param name="target">The target type that may define the member is required.</param>
        /// <param name="member">The member to check on whether it is required by the target type or not.</param>
        /// <returns>True if the member is required by the type. False otherwise.</returns>
        bool IsMemberRequired(Type target, ISerializableMember member);

        /// <summary>
        /// Checks whether the provided value is considered valid for the target type.
        /// This is used during deserialization where some types require data to be present, and not null.
        /// </summary>
        /// <param name="target">The target type for which is it needs to pass validation.</param>
        /// <param name="member">The member to check on whether it is valid.</param>
        /// <param name="value">The value to check on whether it is valid.</param>
        /// <returns>True if the value is valid for the member on the type. False otherwise.</returns>
        bool IsValueValid(Type target, ISerializableMember member, object value);
    }
}