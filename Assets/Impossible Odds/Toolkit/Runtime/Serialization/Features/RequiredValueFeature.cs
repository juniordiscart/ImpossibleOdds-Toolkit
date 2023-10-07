using System;
using ImpossibleOdds.Serialization.Caching;

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
        public Type RequiredValueAttribute => typeof(TRequiredValue);

        /// <inheritdoc />
        public bool IsMemberRequired(Type target, ISerializableMember member)
        {
            target.ThrowIfNull(nameof(target));
            member.ThrowIfNull(nameof(member));

            ISerializationReflectionMap typeMap = SerializationUtilities.GetTypeMap(target);
            return typeMap.IsMemberRequired(member.Member, RequiredValueAttribute);
        }

        /// <inheritdoc />
        public bool IsValueValid(Type target, ISerializableMember member, object value)
        {
            target.ThrowIfNull(nameof(target));
            member.ThrowIfNull(nameof(member));

            if (value != null)
            {
                return true;
            }

            ISerializationReflectionMap typeMap = SerializationUtilities.GetTypeMap(target);
            return !typeMap.TryGetRequiredMemberInfo(member.Member, RequiredValueAttribute, out IRequiredSerializableMember requiredAttrInfo) ||
                   !requiredAttrInfo.RequiredParameterAttribute.NullCheck;
        }
    }
}