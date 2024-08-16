using System;
using System.Collections.Generic;
using System.Xml.Linq;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Xml
{
    public class XmlTypeResolutionFeature : IXmlTypeResolutionFeature
    {
        public const string XmlSchemaURL = "http://www.w3.org/2001/XMLSchema-instance";
        public const string XmlSchemaPrefix = "xsi";
        public const string XmlTypeKey = "type";

        /// <inheritdoc />
        public Type TypeResolutionAttribute => typeof(XmlTypeAttribute);

        /// <inheritdoc />
        public XName TypeResolutionKey => XNamespace.Get(XmlSchemaURL) + XmlTypeKey;

        /// <inheritdoc />
        public Type FindTypeInSourceData(Type baseType, XElement sourceData, IXmlSerializationDefinition definition)
        {
            baseType.ThrowIfNull(nameof(baseType));
            sourceData.ThrowIfNull(nameof(sourceData));
            definition.ThrowIfNull(nameof(definition));

            ITypeResolutionParameter[] typeResolutionParameters = SerializationUtilities.GetTypeMap(baseType).GetTypeResolutionParameters(TypeResolutionAttribute);

            Type resolvedType = baseType;

            foreach (ITypeResolutionParameter typeResolutionParameter in typeResolutionParameters)
            {
                // If we're considering the same type again, or the type would be a step backwards, then don't bother checking further.
                if ((typeResolutionParameter.Target == resolvedType) ||
                    resolvedType.IsSubclassOf(typeResolutionParameter.Target) ||
                    !baseType.IsAssignableFrom(typeResolutionParameter.Target))
                {
                    continue;
                }

                XmlTypeAttribute xmlTypeAttr = typeResolutionParameter switch
                {
                    XmlTypeAttribute xmlAttr => xmlAttr,
                    IInvertedTypeResolutionParameter inverted => (XmlTypeAttribute)inverted.OriginalParameter,
                    _ => throw new ArgumentOutOfRangeException(typeResolutionParameter.GetType().Name)
                };

                // Fetch the type data from either a defined element or attribute in the source data.
                XName processedKey = xmlTypeAttr.KeyOverride ?? TypeResolutionKey;
                string sourceTypeValue = xmlTypeAttr.SetAsElement switch
                {
                    true when sourceData.HasElements => sourceData.Element(processedKey)?.Value,
                    false when sourceData.HasAttributes => sourceData.Attribute(processedKey)?.Value,
                    _ => null
                };

                // If no such source value could be found, then skip it.
                if (sourceTypeValue is null)
                {
                    continue;
                }

                // Compare the values.
                object typeParameterValue = typeResolutionParameter.Value ?? typeResolutionParameter.Target.Name;
                string processedValue = SerializationUtilities.PostProcessValue<string>(Serializer.Serialize(typeParameterValue, definition));
                if (!Equals(sourceTypeValue, processedValue))
                {
                    continue;
                }

                resolvedType = typeResolutionParameter.Target;
            }

            // If a different type was found other than the original, then attempt to search further.
            if (baseType != resolvedType)
            {
                resolvedType = FindTypeInSourceData(resolvedType, sourceData, definition);
            }

            return resolvedType;
        }

        /// <inheritdoc />
        public void InsertTypeInData(Type sourceType, XElement serializedData, IXmlSerializationDefinition definition)
        {
            sourceType.ThrowIfNull(nameof(sourceType));
            serializedData.ThrowIfNull(nameof(serializedData));
            definition.ThrowIfNull(nameof(definition));

            ITypeResolutionParameter[] typeResolutionParameters = SerializationUtilities.GetTypeMap(sourceType).GetTypeResolutionParameters(TypeResolutionAttribute);
            Dictionary<object, Type> insertedTypeInfo = new Dictionary<object, Type>();

            foreach (ITypeResolutionParameter typeResolutionParameter in typeResolutionParameters)
            {
                if (!typeResolutionParameter.Target.IsAssignableFrom(sourceType))
                {
                    continue;
                }

                XmlTypeAttribute xmlTypeAttr = typeResolutionParameter switch
                {
                    XmlTypeAttribute xmlAttr => xmlAttr,
                    IInvertedTypeResolutionParameter inverted => (XmlTypeAttribute)inverted.OriginalParameter,
                    _ => throw new ArgumentOutOfRangeException(typeResolutionParameter.GetType().Name)
                };

                XName typeKey = xmlTypeAttr.KeyOverride ?? TypeResolutionKey;

                // If the information was already present before this function added the type information,
                // then the type information is assumed to be part of the object's serialized data already and won't be modified.
                // If it was added by this function, then it should check that the type information of the most closely related type is used.
                bool typeDataAlreadyPresent = xmlTypeAttr.SetAsElement ? (serializedData.Element(typeKey) != null) : (serializedData.Attribute(typeKey) != null);
                if ((!insertedTypeInfo.ContainsKey(typeKey) && typeDataAlreadyPresent) ||
                    insertedTypeInfo.ContainsKey(typeKey) && !insertedTypeInfo[typeKey].IsAssignableFrom(typeResolutionParameter.Target))
                {
                    continue;
                }

                object typeValue = typeResolutionParameter.Value ?? typeResolutionParameter.Target.Name;
                insertedTypeInfo[typeKey] = typeResolutionParameter.Target;

                if (xmlTypeAttr.SetAsElement)
                {
                    typeValue = Serializer.Serialize(typeValue, definition);
                    serializedData.Add(new XElement(typeKey, typeValue));
                }
                else
                {
                    typeValue = Serializer.Serialize(typeValue, definition.AttributeSerializationDefinition);
                    typeValue = SerializationUtilities.PostProcessValue<string>(typeValue);
                    serializedData.SetAttributeValue(typeKey, typeValue);
                }
            }
        }
    }
}
