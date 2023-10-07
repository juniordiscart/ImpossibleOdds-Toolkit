using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Xml
{
    public interface IXmlSerializationDefinition : ISerializationDefinition
    {
        /// <summary>
        /// The serialization definition used specifically for processing XML attributes.
        /// </summary>
        public ISerializationDefinition AttributeSerializationDefinition { get; }

        /// <summary>
        /// The serialization definition used specifically for processing XML CDATA sections.
        /// </summary>
        public ISerializationDefinition CDataSerializationDefinition { get; }
    }

}