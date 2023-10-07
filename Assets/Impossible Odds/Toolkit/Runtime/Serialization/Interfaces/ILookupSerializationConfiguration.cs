using System;
using System.Collections;
using ImpossibleOdds.Serialization.Caching;

namespace ImpossibleOdds.Serialization
{
    /// <summary>
    /// Interface for configuring the attributes and working data structures to process to and from a dictionary.
    /// </summary>
    public interface ILookupSerializationConfiguration
    {
        /// <summary>
        /// Type of the attribute to be placed atop of a class or struct to denote that the object should be serialized as a lookup data structure.
        /// </summary>
        Type TypeMarkingAttribute
        {
            get;
        }

        /// <summary>
        /// Type of the attribute to be placed atop of the data members to state that they should be picked up in the lookup data structure.
        /// </summary>
        Type MemberAttribute
        {
            get;
        }

        /// <summary>
        /// Creates a lookup data structure to process the data and that works for the result serialization result.
        /// </summary>
        /// <param name="capacity">The capacity of the dictionary to be created.</param>
        /// <returns>A dictionary for processing serialization data.</returns>
        IDictionary CreateLookupInstance(int capacity);

        /// <summary>
        /// Get the lookup key for the provided serializable member.
        /// </summary>
        /// <param name="member">The member for which to get the lookup key.</param>
        /// <returns>The lookup key.</returns>
        object GetLookupKey(ISerializableMember member);
    }
}