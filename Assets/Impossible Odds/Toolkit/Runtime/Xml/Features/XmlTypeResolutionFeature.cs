using System;
using System.Collections.Generic;
using System.Linq;
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

			ITypeResolutionParameter[] typeResolutionParameters =  SerializationUtilities.GetTypeMap(baseType).GetTypeResolveParameters(TypeResolutionAttribute);

			Type resolvedType = baseType;

			foreach (XmlTypeAttribute typeResolutionParameter in typeResolutionParameters.Where(trp => trp is XmlTypeAttribute).Cast<XmlTypeAttribute>())
			{
				// If we're considering the same type again, or the type would be a step backwards, then don't bother checking further.
				if ((typeResolutionParameter.Target == resolvedType) ||
				    resolvedType.IsSubclassOf(typeResolutionParameter.Target) ||
				    !baseType.IsAssignableFrom(typeResolutionParameter.Target))
				{
					continue;
				}

				// Fetch the type data from either a defined element or attribute in the source data.
				XName processedKey = typeResolutionParameter.KeyOverride ?? TypeResolutionKey;
				string sourceTypeValue = typeResolutionParameter.SetAsElement switch
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

	        ITypeResolutionParameter[] typeResolutionParameters = SerializationUtilities.GetTypeMap(sourceType).GetTypeResolveParameters(TypeResolutionAttribute);
	        Dictionary<object, Type> insertedTypeInfo = new Dictionary<object, Type>();

	        foreach (XmlTypeAttribute typeResolutionParameter in typeResolutionParameters.Where(trp => trp is XmlTypeAttribute).Cast<XmlTypeAttribute>())
	        {
		        if (!typeResolutionParameter.Target.IsAssignableFrom(sourceType))
		        {
			        continue;
		        }

		        XName typeKey = typeResolutionParameter.KeyOverride ?? TypeResolutionKey;

		        // If the information was already present before this function added the type information, then the type information
		        // is assumed to be part of the object's serialized data already. If it was added by this function, then it should check
		        // that the information from the most basic available type is used.
		        if (typeResolutionParameter.SetAsElement)
		        {
			        if ((!insertedTypeInfo.ContainsKey(typeKey) && (serializedData.Element(typeKey) != null)) ||
			            insertedTypeInfo.ContainsKey(typeKey) && insertedTypeInfo[typeKey].IsAssignableFrom(typeResolutionParameter.Target))
			        {
				        continue;
			        }
		        }
		        else
		        {
			        if ((!insertedTypeInfo.ContainsKey(typeKey) && (serializedData.Attribute(typeKey) != null)) ||
			            insertedTypeInfo.ContainsKey(typeKey) && insertedTypeInfo[typeKey].IsAssignableFrom(typeResolutionParameter.Target))
			        {
				        continue;
			        }
		        }

		        object typeValue = typeResolutionParameter.Value ?? typeResolutionParameter.Target.Name;

		        if (typeResolutionParameter.SetAsElement)
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

		        insertedTypeInfo[typeKey] = typeResolutionParameter.Target;
	        }
        }
    }
}