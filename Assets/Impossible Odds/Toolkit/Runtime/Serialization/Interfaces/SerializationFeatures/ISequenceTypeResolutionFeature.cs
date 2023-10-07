using System;
using System.Collections;

namespace ImpossibleOdds.Serialization
{
    /// <summary>
    /// Interface for index-based type resolution.
    /// </summary>
    public interface ISequenceTypeResolutionFeature : ITypeResolutionFeature
    {
        /// <summary>
        /// The index in the sequence at which the type information can be found.
        /// </summary>
        int TypeResolutionIndex
        {
            get;
        }

        /// <summary>
        /// Attempts to further refine the target type based the source data.
        /// </summary>
        /// <param name="targetType">The type from which to start the search.</param>
        /// <param name="sourceData">The data in which to search for additional type data.</param>
        /// <param name="definition">The serialization definition that's used to process indices.</param>
        /// <returns>A more refined type based on the source data. If nothing could be found, the target type is returned again instead.</returns>
        Type FindTypeInSourceData(Type targetType, IList sourceData, ISerializationDefinition definition);

        /// <summary>
        /// Insert the type data in the serialized data.
        /// </summary>
        /// <param name="sourceType">The type of object that is serialized.</param>
        /// <param name="serializedData">The serialized object data.</param>
        /// <param name="definition">The serialization definition to process the type value into a compatible value.</param>
        void InsertTypeInData(Type sourceType, IList serializedData, ISerializationDefinition definition);
    }
}