using System;
using System.Collections;
using ImpossibleOdds.Serialization.Caching;

namespace ImpossibleOdds.Serialization
{
    /// <summary>
    /// Generic serialization configuration for processors that require lookup data structure support.
    /// </summary>
    /// <typeparam name="TMarking">The type of the attribute to be placed atop of a class or struct to define that the object should be processed into a lookup data structure.</typeparam>
    /// <typeparam name="TMember">The type of the attribute to be placed atop of the data members of the object.</typeparam>
    public class LookupSerializationConfiguration<TMarking, TMember> : ILookupSerializationConfiguration
        where TMarking : Attribute, ILookupTypeObject
        where TMember : Attribute, ILookupParameter
    {
        private Func<int, IDictionary> LookupGenerator { get; }

        public LookupSerializationConfiguration(Func<int, IDictionary> lookupGenerator)
        {
            lookupGenerator.ThrowIfNull(nameof(lookupGenerator));
            LookupGenerator = lookupGenerator;
        }

        /// <inheritdoc />
        public Type TypeMarkingAttribute => typeof(TMarking);

        /// <inheritdoc />
        public Type MemberAttribute => typeof(TMember);

        /// <inheritdoc />
        public IDictionary CreateLookupInstance(int capacity)
        {
            return LookupGenerator(capacity);
        }

        /// <inheritdoc />
        public object GetLookupKey(ISerializableMember member)
        {
            return ((ILookupParameter)member.Attribute).Key ?? member.Member.Name;
        }
    }
}