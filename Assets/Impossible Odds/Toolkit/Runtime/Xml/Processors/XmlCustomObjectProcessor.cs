using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using ImpossibleOdds.Serialization;
using ImpossibleOdds.Serialization.Caching;
using ImpossibleOdds.Serialization.Processors;

namespace ImpossibleOdds.Xml.Processors
{
    public class XmlCustomObjectProcessor : AbstractCustomObjectProcessor, IDeserializationToTargetProcessor
    {
        public bool ParallelProcessingEnabled => ParallelProcessingFeature is { Enabled: true };
        public bool SupportsRequiredValues => RequiredValueFeature != null;
        public IParallelProcessingFeature ParallelProcessingFeature { get; set; }
        public IRequiredValueFeature RequiredValueFeature { get; set; }
        public IXmlTypeResolutionFeature TypeResolutionFeature { get; }
        public XmlSerializationDefinition XmlDefinition { get; }

        public XmlCustomObjectProcessor(XmlSerializationDefinition definition, IXmlTypeResolutionFeature typeResolutionFeature)
            : base(definition)
        {
            definition.ThrowIfNull(nameof(definition));
            typeResolutionFeature.ThrowIfNull(nameof(typeResolutionFeature));
            XmlDefinition = definition;
            TypeResolutionFeature = typeResolutionFeature;
        }

        /// <inheritdoc />
        public override object Serialize(object objectToSerialize)
        {
            this.ThrowIfCantSerialize(objectToSerialize);
            
            if (objectToSerialize == null)
            {
                return null;
            }
            
            XElement element = new XElement("Element"); // At this stage, we don't know the name yet.

            InvokeOnSerializationCallback(objectToSerialize, element);
            object serializedResult = Serialize(objectToSerialize, element);
            InvokeOnSerializedCallback(objectToSerialize, element);
            return serializedResult;
        }

        /// <inheritdoc />
        public override object Deserialize(Type targetType, object dataToDeserialize)
        {
            this.ThrowIfCantDeserialize(targetType, dataToDeserialize);

            Type instanceType = TypeResolutionFeature.FindTypeInSourceData(targetType, (XElement)dataToDeserialize, XmlDefinition);
            object targetInstance = SerializationUtilities.CreateInstance(instanceType);
            Deserialize(targetInstance, dataToDeserialize);
            return targetInstance;
        }
        
        /// <inheritdoc />
        public virtual void Deserialize(object deserializationTarget, object dataToDeserialize)
        {
            this.ThrowIfCantDeserializeToTarget(deserializationTarget, dataToDeserialize);

            // If the source value is null, then there is little to do.
            if (dataToDeserialize == null)
            {
                return;
            }

            InvokeOnDeserializationCallback(deserializationTarget, dataToDeserialize);
            Deserialize(deserializationTarget, (XElement)dataToDeserialize);
            InvokeOnDeserializedCallback(deserializationTarget, dataToDeserialize);
        }

        /// <inheritdoc />
        public override bool CanSerialize(object objectToSerialize)
        {
            return
                (objectToSerialize == null) ||
                Attribute.IsDefined(objectToSerialize.GetType(), typeof(XmlObjectAttribute), true);
        }

        /// <inheritdoc />
        public override bool CanDeserialize(Type targetType, object dataToDeserialize)
        {
            targetType.ThrowIfNull(nameof(targetType));

            if (dataToDeserialize == null)
            {
                return SerializationUtilities.IsNullableType(targetType);
            }

            if (dataToDeserialize is not XElement xElement)
            {
                return false;
            }

            Type instanceType = TypeResolutionFeature.FindTypeInSourceData(targetType, xElement, XmlDefinition);
            return Attribute.IsDefined(instanceType, typeof(XmlObjectAttribute), true);
        }

        /// <inheritdoc />
        public bool CanDeserialize(object deserializationTarget, object dataToDeserialize)
        {
            return deserializationTarget != null && CanDeserialize(deserializationTarget.GetType(), dataToDeserialize);
        }

        private XElement Serialize(object source, XElement element)
        {
            Type sourceType = source.GetType();
            ISerializationReflectionMap sourceTypeCache = SerializationUtilities.GetTypeMap(sourceType);
            ISerializableMember[] elementFields = sourceTypeCache.GetUniqueSerializableMembers(typeof(AbstractXmlMemberAttribute));

            if (ParallelProcessingEnabled)
            {
                // Since there's no logical way of determining the order in which values are processed
                // in parallel for XML, the results are cached in an array, which is applied to the
                // element in sequence.
                XObject[] resultsCache = new XObject[elementFields.Length];
                Parallel.For(0, resultsCache.Length, index =>
                {
                    XObject serializedMember = SerializeMember(elementFields[index]);
                    resultsCache[index] = serializedMember; // No need to lock because of fixed array size.
                });

                foreach (XObject xObject in resultsCache)
                {
                    element.Add(xObject);
                }
            }
            else
            {
                Array.ForEach(elementFields, sourceMember =>
                {
                    element.Add(SerializeMember(sourceMember));
                });
            }

            // Find a type resolution parameter, if any. If none are available, then this element is done.
            XmlTypeAttribute typeResolveParameter = ResolveTypeForSerialization(sourceType);
            if (typeResolveParameter == null)
            {
                return element;
            }

            // Find a type resolution key. If information is already filled
            // in the expected place, then this element is already done.
            XName typeKey = typeResolveParameter.KeyOverride ?? TypeResolutionFeature.TypeResolutionKey;
            if ((element.Element(typeKey) != null) || (element.Attribute(typeKey) != null))
            {
                return element;
            }

            // Add the type information either as an element, or an attribute.
            object typeValue = typeResolveParameter.Value ?? typeResolveParameter.Target.Name;
            if (typeResolveParameter.SetAsElement)
            {
                typeValue = Serializer.Serialize(typeValue, Definition);
                element.Add(new XElement(typeKey, typeValue));
            }
            else
            {
                typeValue = Serializer.Serialize(typeValue, XmlDefinition.AttributeSerializationDefinition);
                typeValue = SerializationUtilities.PostProcessValue<string>(typeValue);
                element.SetAttributeValue(typeKey, typeValue);
            }

            return element;

            XObject SerializeMember(ISerializableMember sourceMember)
            {
                object value = sourceMember.GetValue(source);

                return sourceMember.Attribute switch
                {
                    XmlAttributeAttribute attributeAttribute => Serialize(value, sourceMember.Member, attributeAttribute),
                    XmlElementAttribute elementAttribute => Serialize(value, sourceMember.Member, elementAttribute),
                    XmlListElementAttribute listElementAttribute => Serialize(value, sourceMember.Member, listElementAttribute),
                    XmlCDataAttribute cdataElementAttribute => Serialize(value, sourceMember.Member, cdataElementAttribute),
                    _ => throw new XmlException($"Unsupported XML serialization attribute of type {sourceMember.Attribute.GetType().Name}.")
                };
            }
        }

        private XElement Serialize(object fieldValue, MemberInfo memberInfo, XmlElementAttribute elementAttribute)
        {
            fieldValue = Serializer.Serialize(fieldValue, Definition);

            if (fieldValue is not XElement xmlElement)
            {
                return new XElement(GetElementKey(elementAttribute, memberInfo), fieldValue);
            }

            xmlElement.Name = GetElementKey(elementAttribute, memberInfo);
            return xmlElement;
        }

        private XAttribute Serialize(object fieldValue, MemberInfo memberInfo, XmlAttributeAttribute attributeAttribute)
        {
            return new XAttribute(GetElementKey(attributeAttribute, memberInfo), Serializer.Serialize(fieldValue, XmlDefinition.AttributeSerializationDefinition));
        }

        private XElement Serialize(object fieldValue, MemberInfo memberInfo, XmlListElementAttribute listElementAttribute)
        {
            if (fieldValue is not IList)
            {
                throw new XmlException($"The value of member {memberInfo.Name} of type {memberInfo.DeclaringType?.Name} is requested to be serialized as a list, but does not implement the {nameof(IList)} interface.");
            }

            fieldValue = Serializer.Serialize(fieldValue, Definition);

            // If the serialized field is not an XML element, then we can't continue.
            if (!(fieldValue is XElement xmlElement))
            {
                throw new XmlException($"The serialized result of member {memberInfo.Name} of type {memberInfo.DeclaringType?.Name} did not return a valid {nameof(XElement)} result.");
            }

            xmlElement.Name = GetElementKey(listElementAttribute, memberInfo);
            foreach (XElement childElement in xmlElement.Elements())
            {
                childElement.Name = listElementAttribute.EntryName;
            }

            return xmlElement;
        }

        private XElement Serialize(object fieldValue, MemberInfo memberInfo, XmlCDataAttribute cdataAttribute)
        {
            XElement cdataParent = new XElement(GetElementKey(cdataAttribute, memberInfo));

            if (fieldValue != null)
            {
                cdataParent.Add(new XCData((string)Serializer.Serialize(fieldValue, XmlDefinition.CDataSerializationDefinition)));
            }

            return cdataParent;
        }

        private void Deserialize(object target, XElement source)
        {
            // Process child elements and attributes. Don't bother if it doesn't have any.
            if (!source.HasElements && !source.HasAttributes)
            {
                return;
            }

            Type targetType = target.GetType();
            ISerializationReflectionMap typeMap = SerializationUtilities.GetTypeMap(targetType);
            ISerializableMember[] members = typeMap.GetSerializableMembers(typeof(AbstractXmlMemberAttribute));

            if (XmlDefinition.ParallelProcessingEnabled && (members.Length > 1))
            {
                Parallel.ForEach(members, member =>
                {
                    object result = DeserializeMember(member);
                    lock (target) member.SetValue(target, result);
                });
            }
            else
            {
                Array.ForEach(members, member =>
                {
                    member.SetValue(target, DeserializeMember(member));
                });
            }

            return;

            object DeserializeMember(ISerializableMember targetMember)
            {
                object result = targetMember.Attribute switch
                {
                    XmlAttributeAttribute attributeAttribute => Deserialize(source, targetMember, attributeAttribute, typeMap),
                    XmlElementAttribute elementAttribute => Deserialize(source, targetMember, elementAttribute, typeMap),
                    XmlListElementAttribute listElementAttribute => Deserialize(source, targetMember, listElementAttribute, typeMap),
                    XmlCDataAttribute cdataElementAttribute => Deserialize(source, targetMember, cdataElementAttribute, typeMap),
                    _ => throw new XmlException($"Unsupported XML deserialization attribute of type {targetMember.Attribute.GetType().Name}.")
                };

                if (result != null)
                {
                    return result;
                }

                if (SupportsRequiredValues && !RequiredValueFeature.IsValueValid(targetType, targetMember, null))
                {
                    throw new XmlException($"The member '{targetMember.Member.Name}' is marked as required on type {targetMember.Member.DeclaringType?.Name} but the value is null in the source.");
                }

                Type memberType = targetMember.MemberType;
                return memberType.IsValueType ? Activator.CreateInstance(memberType, true) : null;
            }
        }

        private object Deserialize(XElement source, ISerializableMember memberInfo, XmlAttributeAttribute attributeInfo, ISerializationReflectionMap typeMap)
        {
            XAttribute attribute = source.Attribute(GetElementKey(attributeInfo, memberInfo.Member));
            if (attribute != null)
            {
                return Serializer.Deserialize(memberInfo.MemberType, attribute.Value, XmlDefinition.AttributeSerializationDefinition);
            }

            if (SupportsRequiredValues && typeMap.IsMemberRequired(memberInfo.Member, RequiredValueFeature.RequiredValueAttribute))
            {
                throw new XmlException($"The member '{memberInfo.Member.Name}' is marked as required on type {memberInfo.Member.DeclaringType?.Name} but is not present in the source.");
            }

            return SerializationUtilities.GetDefaultValue(memberInfo.MemberType);
        }

        private object Deserialize(XElement source, ISerializableMember memberInfo, XmlElementAttribute elementInfo, ISerializationReflectionMap typeMap)
        {
            XElement childElement = source.Element(GetElementKey(elementInfo, memberInfo.Member));
            if (childElement != null)
            {
                return Serializer.Deserialize(memberInfo.MemberType, childElement, Definition);
            }

            if (SupportsRequiredValues && typeMap.IsMemberRequired(memberInfo.Member, RequiredValueFeature.RequiredValueAttribute))
            {
                throw new XmlException($"The member '{memberInfo.Member.Name}' is marked as required on type {memberInfo.Member.DeclaringType?.Name} but is not present in the source.");
            }

            return SerializationUtilities.GetDefaultValue(memberInfo.MemberType);
        }

        private object Deserialize(XElement source, ISerializableMember memberInfo, XmlListElementAttribute listElementInfo, ISerializationReflectionMap typeMap)
        {
            if (!typeof(IList).IsAssignableFrom(memberInfo.MemberType))
            {
                throw new XmlException($"Member {memberInfo.Member.Name} of type {memberInfo.Member.DeclaringType?.Name} is marked as an XML List, but does not implement any {nameof(IList)} interface to receive these values.");
            }

            XElement childElement = source.Element(GetElementKey(listElementInfo, memberInfo.Member));
            if (childElement != null)
            {
                return Serializer.Deserialize(memberInfo.MemberType, childElement, Definition);
            }

            if (SupportsRequiredValues && typeMap.IsMemberRequired(memberInfo.Member, RequiredValueFeature.RequiredValueAttribute))
            {
                throw new XmlException($"The member '{memberInfo.Member.Name}' is marked as required on type {memberInfo.Member.DeclaringType?.Name} but is not present in the source.");
            }

            return SerializationUtilities.GetDefaultValue(memberInfo.MemberType);
        }

        private object Deserialize(XElement source, ISerializableMember memberInfo, XmlCDataAttribute cdataInfo, ISerializationReflectionMap typeMap)
        {
            XElement childElement = source.Element(GetElementKey(cdataInfo, memberInfo.Member));
            if (childElement == null)
            {
                if (SupportsRequiredValues && typeMap.IsMemberRequired(memberInfo.Member, RequiredValueFeature.RequiredValueAttribute))
                {
                    throw new XmlException($"The member '{memberInfo.Member.Name}' is marked as required on type {memberInfo.Member.DeclaringType?.Name} but is not present in the source.");
                }

                return SerializationUtilities.GetDefaultValue(memberInfo.MemberType);
            }

            // Find the first CDATA node in the child elements. Formatted XML documents may contain whitespace elements before the CDATA section is actually reached.
            foreach (XNode node in childElement.Nodes())
            {
                if (node is XCData cdata)
                {
                    return Serializer.Deserialize(memberInfo.MemberType, cdata.Value, XmlDefinition.CDataSerializationDefinition);
                }
            }

            return SerializationUtilities.GetDefaultValue(memberInfo.MemberType);
        }

        private static string GetElementKey(AbstractXmlMemberAttribute xmlAttribute, MemberInfo member)
        {
            return !string.IsNullOrWhiteSpace(xmlAttribute.Key) ? xmlAttribute.Key : member.Name;
        }

        private static XmlTypeAttribute ResolveTypeForSerialization(Type sourceType)
        {
            ITypeResolutionParameter[] typeResolveAttributes = SerializationUtilities.GetTypeMap(sourceType).GetTypeResolutionParameters(typeof(XmlTypeAttribute));

            // Find the attribute with the right target.
            return typeResolveAttributes.Where(tr => tr.Target == sourceType).Cast<XmlTypeAttribute>().FirstOrDefault();
        }
    }
}
