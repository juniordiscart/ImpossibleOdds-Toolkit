using System;
using System.Xml.Linq;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Xml
{
    public interface IXmlTypeResolutionFeature : ITypeResolutionFeature
    {
        /// <summary>
        /// The key where the type information will be placed at in the XML element.
        /// </summary>
        XName TypeResolutionKey
        {
            get;
        }

        /// <summary>
        /// Provided with a type and source data, attempt to extract a more defined type.
        /// This is useful during deserialization, where the most defined type is requested to instantiate
        /// a new instance of an object onto which the data is deserialized.
        /// </summary>
        /// <param name="baseType">The type from which to start the search to a more defined type.</param>
        /// <param name="sourceData">The data in which to search for the type information.</param>
        /// <param name="definition">The serialization definition that's used to process keys.</param>
        /// <returns>The most defined type that could be resolved from the data for the base type.</returns>
        Type FindTypeInSourceData(Type baseType, XElement sourceData, IXmlSerializationDefinition definition);

        /// <summary>
        /// Insert the type data in the serialized data.
        /// </summary>
        /// <param name="sourceType">The type of object that is serialized.</param>
        /// <param name="serializedData">The serialized object data.</param>
        /// <param name="definition">The serialization definition to process the type key and value into compatible values.</param>
        void InsertTypeInData(Type sourceType, XElement serializedData, IXmlSerializationDefinition definition);
    }
}