using System;
using System.Collections;
using ImpossibleOdds.Serialization.Caching;
using UnityEngine;

namespace ImpossibleOdds.Serialization
{
    public class SequenceSerializationConfiguration<TMarking, TMember> : ISequenceSerializationConfiguration
        where TMarking : Attribute, ISequenceTypeObject
    {
        private Func<int, IList> SequenceGenerator { get; }

        public SequenceSerializationConfiguration(Func<int, IList> sequenceGenerator)
        {
            sequenceGenerator.ThrowIfNull(nameof(sequenceGenerator));
            SequenceGenerator = sequenceGenerator;
        }

        /// <inheritdoc />
        public Type TypeMarkingAttribute => typeof(TMarking);

        /// <inheritdoc />
        public Type MemberAttribute => typeof(TMember);

        /// <inheritdoc />
        public IList CreateSequenceInstance(int capacity)
        {
            return SequenceGenerator(capacity);
        }

        /// <inheritdoc />
        public int GetIndex(ISerializableMember member)
        {
            return ((ISequenceParameter)member.Attribute).Index;
        }

        /// <inheritdoc />
        public int GetMaxDefinedIndex(Type type)
        {
            int maxIndex = int.MinValue;
            ISerializableMember[] members = SerializationUtilities.GetTypeMap(type).GetSerializableMembers(MemberAttribute);

            foreach (ISerializableMember member in members)
            {
                maxIndex = Mathf.Max(maxIndex, ((ISequenceParameter)member.Attribute).Index);
            }

            return maxIndex;
        }
    }
}