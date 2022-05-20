namespace ImpossibleOdds.Xml.Processors
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Xml.Linq;
	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Serialization.Caching;
	using ImpossibleOdds.Serialization.Processors;

	public class XmlCustomObjectProcessor : AbstractCustomObjectProcessor, IDeserializationToTargetProcessor
	{
		private XmlSerializationDefinition xmlDefinition = null;

		public XmlSerializationDefinition XmlDefinition
		{
			get { return xmlDefinition; }
		}

		public XmlCustomObjectProcessor(XmlSerializationDefinition definition) : base(definition)
		{
			definition.ThrowIfNull(nameof(definition));
			xmlDefinition = definition;
		}

		public override bool Serialize(object objectToSerialize, out object serializedResult)
		{
			if (objectToSerialize == null)
			{
				serializedResult = objectToSerialize;
				return true;
			}

			Type sourceType = objectToSerialize.GetType();
			if (!sourceType.IsDefined(typeof(XmlObjectAttribute), true))
			{
				serializedResult = null;
				return false;
			}

			InvokeOnSerializationCallback(objectToSerialize);
			serializedResult = Serialize(sourceType, objectToSerialize);
			InvokeOnSerializedCallback(objectToSerialize);
			return true;
		}

		public override bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult)
		{
			targetType.ThrowIfNull(nameof(targetType));

			if (dataToDeserialize == null)
			{
				deserializedResult = null;
				return SerializationUtilities.IsNullableType(targetType);
			}
			else if (!(dataToDeserialize is XElement))
			{
				deserializedResult = null;
				return false;
			}

			Type instanceType = ResolveTypeForDeserialization(targetType, dataToDeserialize as XElement);
			if (!instanceType.IsDefined(typeof(XmlObjectAttribute), true))
			{
				deserializedResult = null;
				return false;
			}

			object targetInstance = SerializationUtilities.CreateInstance(instanceType);

			if (Deserialize(targetInstance, dataToDeserialize))
			{
				deserializedResult = targetInstance;
				return true;
			}
			else
			{
				deserializedResult = null;
				return false;
			}
		}

		public bool Deserialize(object deserializationTarget, object dataToDeserialize)
		{
			deserializationTarget.ThrowIfNull(nameof(deserializationTarget));

			// If the source value is null, then there is little to do.
			if (dataToDeserialize == null)
			{
				return true;
			}
			else if (!deserializationTarget.GetType().IsDefined(typeof(XmlObjectAttribute), true))
			{
				return false;
			}

			InvokeOnDeserializationCallback(deserializationTarget);
			Deserialize(deserializationTarget, dataToDeserialize as XElement);
			InvokeOnDeserializedCallback(deserializationTarget);
			return true;
		}

		private XElement Serialize(Type sourceType, object source)
		{
			XElement element = new XElement("Element");  // At this stage, we don't know the name yet.
			SerializationTypeMap sourceTypeCache = SerializationUtilities.GetTypeMap(sourceType);

			// Process child elements.
			IReadOnlyList<IMemberAttributeTuple> elementFields = sourceTypeCache.GetMembersWithAttribute(typeof(AbstractXmlMemberAttribute));
			foreach (IMemberAttributeTuple elementField in elementFields)
			{
				object value = elementField.GetValue(source);

				XObject xmlResult = null;
				if (elementField.Attribute is XmlAttributeAttribute attributeAttribute)
				{
					xmlResult = Serialize(value, elementField.Member, attributeAttribute);
				}
				else if (elementField.Attribute is XmlElementAttribute elementAttribute)
				{
					xmlResult = Serialize(value, elementField.Member, elementAttribute);
				}
				else if (elementField.Attribute is XmlListElementAttribute listElementAttribute)
				{
					xmlResult = Serialize(value, elementField.Member, listElementAttribute);
				}
				else if (elementField.Attribute is XmlCDataAttribute cdataElementAttribute)
				{
					xmlResult = Serialize(value, elementField.Member, cdataElementAttribute);
				}
				else
				{
					throw new XmlException("Unsupported XML serialization attribute of type {0}.", elementField.Attribute.GetType().Name);
				}

				element.Add(xmlResult);
			}

			// Add the type information, if any.
			XmlTypeAttribute typeResolveParameter = ResolveTypeForSerialization(sourceType);
			if (typeResolveParameter != null)
			{
				XName typeKey = (typeResolveParameter.KeyOverride != null) ? typeResolveParameter.KeyOverride : xmlDefinition.TypeResolveKey;
				if ((element.Element(typeKey) == null) && (element.Attribute(typeKey) == null))
				{
					object typeValue = (typeResolveParameter.Value != null) ? typeResolveParameter.Value : typeResolveParameter.Target.Name;
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
				}
			}

			return element;
		}

		private XElement Serialize(object fieldValue, MemberInfo memberInfo, XmlElementAttribute elementAttribute)
		{
			fieldValue = Serializer.Serialize(fieldValue, Definition);

			if (fieldValue is XElement xmlElement)
			{
				xmlElement.Name = GetElementKey(elementAttribute, memberInfo);
				return xmlElement;
			}
			else
			{
				return new XElement(GetElementKey(elementAttribute, memberInfo), fieldValue);
			}
		}

		private XAttribute Serialize(object fieldValue, MemberInfo memberInfo, XmlAttributeAttribute attributeAttribute)
		{
			return new XAttribute(GetElementKey(attributeAttribute, memberInfo), Serializer.Serialize(fieldValue, XmlDefinition.AttributeSerializationDefinition));
		}

		private XElement Serialize(object fieldValue, MemberInfo memberInfo, XmlListElementAttribute listElementAttribute)
		{
			if (!(fieldValue is IList listValue))
			{
				throw new XmlException("The value of member {0} of type {1} is requested to be serialized as a list, but does not implement the {2} interface.", memberInfo.Name, memberInfo.DeclaringType.Name, typeof(IList).Name);
			}

			fieldValue = Serializer.Serialize(fieldValue, Definition);

			if (fieldValue is XElement xmlElement)
			{
				xmlElement.Name = GetElementKey(listElementAttribute, memberInfo);
				foreach (XElement childElement in xmlElement.Elements())
				{
					childElement.Name = listElementAttribute.EntryName;
				}

				return xmlElement;
			}
			else
			{
				throw new XmlException("The serialized result of member {0} of type {1} did not return a valid {2} result.", memberInfo.Name, memberInfo.DeclaringType.Name, typeof(XElement).Name);
			}
		}

		private XElement Serialize(object fieldValue, MemberInfo memberInfo, XmlCDataAttribute cdataAttribute)
		{
			XElement cdataParent = new XElement(GetElementKey(cdataAttribute, memberInfo));

			if (fieldValue != null)
			{
				cdataParent.Add(new XCData(Serializer.Serialize(fieldValue, XmlDefinition.CDataSerializationDefinition) as string));
			}

			return cdataParent;
		}

		private void Deserialize(object target, XElement source)
		{
			// Process child elements and attributes. Don't bother if it doesn't have any.
			if (source.HasElements || source.HasAttributes)
			{
				SerializationTypeMap typeMap = SerializationUtilities.GetTypeMap(target.GetType());
				IReadOnlyList<IMemberAttributeTuple> members = typeMap.GetMembersWithAttribute(typeof(AbstractXmlMemberAttribute));
				foreach (IMemberAttributeTuple member in members)
				{
					object processedResult = null;

					if (member.Attribute is XmlAttributeAttribute attributeAttribute)
					{
						processedResult = Deserialize(source, member, attributeAttribute, typeMap);
					}
					else if (member.Attribute is XmlElementAttribute elementAttribute)
					{
						processedResult = Deserialize(source, member, elementAttribute, typeMap);
					}
					else if (member.Attribute is XmlListElementAttribute listElementAttribute)
					{
						processedResult = Deserialize(source, member, listElementAttribute, typeMap);
					}
					else if (member.Attribute is XmlCDataAttribute cdataElementAttribute)
					{
						processedResult = Deserialize(source, member, cdataElementAttribute, typeMap);
					}
					else
					{
						throw new XmlException("Unsupported XML deserialization attribute of type {0}.", member.Attribute.GetType().Name);
					}

					if ((processedResult == null) && typeMap.IsMemberRequired(member.Member, xmlDefinition, out IRequiredMemberAttributeTuple requiredAttr) && requiredAttr.RequiredParameterAttribute.NullCheck)
					{
						throw new XmlException("The member '{0}' is marked as required on type {1} but the value is null in the source.", member.Member.Name, member.Member.DeclaringType.Name);
					}

					member.SetValue(target, processedResult);
				}
			}
		}

		private object Deserialize(XElement source, IMemberAttributeTuple memberInfo, XmlAttributeAttribute attributeInfo, SerializationTypeMap typeMap)
		{
			XAttribute attribute = source.Attribute(GetElementKey(attributeInfo, memberInfo.Member));
			if (attribute != null)
			{
				return Serializer.Deserialize(memberInfo.MemberType, attribute.Value, XmlDefinition.AttributeSerializationDefinition);
			}
			else if (typeMap.IsMemberRequired(memberInfo.Member, xmlDefinition))
			{
				throw new XmlException("The member '{0}' is marked as required on type {1} but is not present in the source.", memberInfo.Member.Name, memberInfo.Member.DeclaringType.Name);
			}
			else
			{
				return SerializationUtilities.GetDefaultValue(memberInfo.MemberType);
			}
		}

		private object Deserialize(XElement source, IMemberAttributeTuple memberInfo, XmlElementAttribute elementInfo, SerializationTypeMap typeMap)
		{
			XElement childElement = source.Element(GetElementKey(elementInfo, memberInfo.Member));
			if (childElement != null)
			{
				return Serializer.Deserialize(memberInfo.MemberType, (childElement.HasAttributes || childElement.HasElements) ? (object)childElement : (object)childElement.Value, Definition);
			}
			else if (typeMap.IsMemberRequired(memberInfo.Member, xmlDefinition))
			{
				throw new XmlException("The member '{0}' is marked as required on type {1} but is not present in the source.", memberInfo.Member.Name, memberInfo.Member.DeclaringType.Name);
			}
			else
			{
				return SerializationUtilities.GetDefaultValue(memberInfo.MemberType);
			}
		}

		private object Deserialize(XElement source, IMemberAttributeTuple memberInfo, XmlListElementAttribute listElementInfo, SerializationTypeMap typeMap)
		{
			if (!typeof(IList).IsAssignableFrom(memberInfo.MemberType))
			{
				throw new XmlException("Member {0} of type {1} is marked as an XML List, but does not implement any {2} interface to receive these values.", memberInfo.Member.Name, memberInfo.Member.DeclaringType.Name, typeof(IList).Name);
			}

			XElement childElement = source.Element(GetElementKey(listElementInfo, memberInfo.Member));
			if (childElement != null)
			{
				return Serializer.Deserialize(memberInfo.MemberType, childElement, Definition);
			}
			else if (typeMap.IsMemberRequired(memberInfo.Member, xmlDefinition))
			{
				throw new XmlException("The member '{0}' is marked as required on type {1} but is not present in the source.", memberInfo.Member.Name, memberInfo.Member.DeclaringType.Name);
			}
			else
			{
				return SerializationUtilities.GetDefaultValue(memberInfo.MemberType);
			}
		}

		private object Deserialize(XElement source, IMemberAttributeTuple memberInfo, XmlCDataAttribute cdataInfo, SerializationTypeMap typeMap)
		{
			XElement childElement = source.Element(GetElementKey(cdataInfo, memberInfo.Member));
			if (childElement == null)
			{
				if (typeMap.IsMemberRequired(memberInfo.Member, xmlDefinition))
				{
					throw new XmlException("The member '{0}' is marked as required on type {1} but is not present in the source.", memberInfo.Member.Name, memberInfo.Member.DeclaringType.Name);
				}
				else
				{
					return SerializationUtilities.GetDefaultValue(memberInfo.MemberType);
				}
			}

			// Find the first CDATA node in the child elements. Formatted XML documents may contain whitespace elements before the CDATA section is actually reached.
			XCData cdata = childElement.Nodes().FirstOrDefault(e => e is XCData) as XCData;
			return (cdata != null) ? Serializer.Deserialize(memberInfo.MemberType, cdata.Value, XmlDefinition.CDataSerializationDefinition) : SerializationUtilities.GetDefaultValue(memberInfo.MemberType);
		}

		private string GetElementKey(AbstractXmlMemberAttribute xmlAttribute, MemberInfo member)
		{
			return !string.IsNullOrWhiteSpace(xmlAttribute.Key) ? xmlAttribute.Key : member.Name;
		}

		private XmlTypeAttribute ResolveTypeForSerialization(Type sourceType)
		{
			IReadOnlyList<ITypeResolveParameter> typeResolveAttributes = SerializationUtilities.GetTypeMap(sourceType).GetTypeResolveParameters(typeof(XmlTypeAttribute));
			return typeResolveAttributes.FirstOrDefault(tr => tr.Target == sourceType) as XmlTypeAttribute;
		}

		private Type ResolveTypeForDeserialization(Type targetType, XElement element)
		{
			IEnumerable<XmlTypeAttribute> typeResolveAttrs =
				SerializationUtilities.GetTypeMap(targetType).
				GetTypeResolveParameters(typeof(XmlTypeAttribute)).
				Cast<XmlTypeAttribute>().
				Where(tra => tra.Target != targetType); // Perform this filter as well as a way to stop the recursion.

			foreach (XmlTypeAttribute typeResolveAttr in typeResolveAttrs)
			{
				Type resolvedTargetType = null;

				// Check if an override key type resolve parameter exists, and use it if a match could be made in the source data.
				if (typeResolveAttr.KeyOverride != null)
				{
					string processedKey = SerializationUtilities.PostProcessValue<string>(Serializer.Serialize(typeResolveAttr.KeyOverride, Definition));
					resolvedTargetType = ResolveTypeForDeserialization(element, typeResolveAttr, processedKey);
				}
				else
				{
					resolvedTargetType = ResolveTypeForDeserialization(element, typeResolveAttr, xmlDefinition.TypeResolveKey);
				}

				// Recursively find a more concrete type, if any.
				if (resolvedTargetType != null)
				{
					if (targetType.IsAssignableFrom(typeResolveAttr.Target))
					{
						return ResolveTypeForDeserialization(resolvedTargetType, element);
					}
					else
					{
						throw new SerializationException("The attribute of type {0}, defined on type {1} or its super types, is matched but cannot be assigned from instance of type {2}.", typeResolveAttr.GetType().Name, targetType.Name, typeResolveAttr.Target.Name);
					}
				}
			}

			return targetType;
		}

		private Type ResolveTypeForDeserialization(XElement element, XmlTypeAttribute typeResolveParam, XName processedKey)
		{
			if (processedKey == null)
			{
				return null;
			}

			if (typeResolveParam.SetAsElement && element.HasElements)
			{
				XElement xmlTypeElement = element.Element(processedKey);
				if (xmlTypeElement != null)
				{
					object value = (typeResolveParam.Value != null) ? typeResolveParam.Value : typeResolveParam.Target.Name;
					string processedValue = SerializationUtilities.PostProcessValue<string>(Serializer.Serialize(value, Definition));
					if (xmlTypeElement.Value.Equals(processedValue))
					{
						return typeResolveParam.Target;
					}
				}
			}
			else if (element.HasAttributes)
			{
				XAttribute xmlTypeAttr = element.Attribute(processedKey);
				if (xmlTypeAttr != null)
				{
					object value = (typeResolveParam.Value != null) ? typeResolveParam.Value : typeResolveParam.Target.Name;
					string processedValue = SerializationUtilities.PostProcessValue<string>(Serializer.Serialize(value, Definition));
					if (xmlTypeAttr.Value.Equals(processedValue))
					{
						return typeResolveParam.Target;
					}
				}
			}

			return null;
		}
	}
}
