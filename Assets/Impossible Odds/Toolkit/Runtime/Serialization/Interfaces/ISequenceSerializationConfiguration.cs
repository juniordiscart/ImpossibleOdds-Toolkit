using System;
using System.Collections;
using ImpossibleOdds.Serialization.Caching;

namespace ImpossibleOdds.Serialization
{
    /// <summary>
    /// Interface for configuring the attributes and working data structures to process to and from a sequence.
    /// </summary>
    public interface ISequenceSerializationConfiguration
    {
        /// <summary>
        /// Type of the attribute to be place atop of a class or struct to denote that the object should be serialized as a sequence.
        /// </summary>
        Type TypeMarkingAttribute
        {
            get;
        }

        /// <summary>
        /// Type of the attribute the placed atop of the data members to state that they should be picked up in the sequence.
        /// </summary>
        Type MemberAttribute
        {
            get;
        }

        /// <summary>
        /// Creates a sequence data structure to process the data and that works for the resulting serialization result.
        /// </summary>
        /// <param name="capacity">The capacity of the sequence to be created.</param>
        /// <returns>A sequence for processing serialization data.</returns>
        IList CreateSequenceInstance(int capacity);

        /// <summary>
        /// Get the index for the provided serializable member.
        /// </summary>
        /// <param name="member">The member for which to get the index.</param>
        /// <returns>The index of the serializable member.</returns>
        int GetIndex(ISerializableMember member);

        /// <summary>
        /// Gets the max defined serialization index for the given type.
        /// </summary>
        /// <param name="type">The type for which to find the maximum index.</param>
        /// <returns>The maximum index found on the type.</returns>
        int GetMaxDefinedIndex(Type type);
    }
}