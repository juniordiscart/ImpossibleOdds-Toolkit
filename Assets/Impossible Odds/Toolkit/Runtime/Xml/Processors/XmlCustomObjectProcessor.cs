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
		public  XmlSerializationDefinition XmlDefinition { get; }

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
			if (objectToSerialize == null)
			{
				return null;
			}

			InvokeOnSerializationCallback(objectToSerialize);
			object serializedResult = Serialize(objectToSerialize.GetType(), objectToSerialize);
			InvokeOnSerializedCallback(objectToSerialize);
			return serializedResult;
		}

		/// <inheritdoc />
		public override object Deserialize(Type targetType, object dataToDeserialize)
		{
			if (!CanDeserialize(targetType, dataToDeserialize))
			{
				throw new SerializationException($"The provided data cannot be deserialized by this processor of type {nameof(XmlCustomObjectProcessor)}.");
			}

			Type instanceType = TypeResolutionFeature.FindTypeInSourceData(targetType, (XElement)dataToDeserialize, XmlDefinition);
			object targetInstance = SerializationUtilities.CreateInstance(instanceType);
			Deserialize(targetInstance, dataToDeserialize);
			return targetInstance;
		}

		/// <inheritdoc />
		public virtual void Deserialize(object deserializationTarget, object dataToDeserialize)
		{
			deserializationTarget.ThrowIfNull(nameof(deserializationTarget));

			// If the source value is null, then there is little to do.
			if (dataToDeserialize == null)
			{
				return;
			}

			InvokeOnDeserializationCallback(deserializationTarget);
			Deserialize(deserializationTarget, (XElement)dataToDeserialize);
			InvokeOnDeserializedCallback(deserializationTarget);
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

			if (!(dataToDeserialize is XElement xElement))
			{
				return false;
			}

			Type instanceType = TypeResolutionFeature.FindTypeInSourceData(targetType, xElement, XmlDefinition);
			return Attribute.IsDefined(instanceType, typeof(XmlObjectAttribute), true);
		}

		private XElement Serialize(Type sourceType, object source)
		{
			XElement element = new XElement("Element");  // At this stage, we don't know the name yet.
			ISerializationReflectionMap sourceTypeCache = SerializationUtilities.GetTypeMap(sourceType);
			ISerializableMember[] elementFields = sourceTypeCache.GetUniqueSerializableMembers(typeof(AbstractXmlMemberAttribute));

			if (ParallelProcessingEnabled)
			{
				// Since there's no logical way of determining the order in which values are processed
				// in parallel for XML, the results are cached in an array, which is applied to the
				// element in sequence.
				XObject[] resultsCache = new XObject[elementFields.Length];
				Parallel.For(0, resultsCache.Length, (int index) =>
				{
					resultsCache[index] = SerializeMember(elementFields[index]);
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
			// in in the expected place, then this element is already done.
			XName typeKey = typeResolveParameter.KeyOverride ?? (XName)TypeResolutionFeature.TypeResolutionKey;
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
					_ => throw new XmlException("Unsupported XML serialization attribute of type {0}.", sourceMember.Attribute.GetType().Name)
				};
			}
		}

		private XElement Serialize(object fieldValue, MemberInfo memberInfo, XmlElementAttribute elementAttribute)
		{
			fieldValue = Serializer.Serialize(fieldValue, Definition);

			if (!(fieldValue is XElement xmlElement))
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
			if (!(fieldValue is IList))
			{
				throw new XmlException("The value of member {0} of type {1} is requested to be serialized as a list, but does not implement the {2} interface.", memberInfo.Name, memberInfo.DeclaringType?.Name, nameof(IList));
			}

			fieldValue = Serializer.Serialize(fieldValue, Definition);

			// If the serialized field is not an XML element, then we can't continue.
			if (!(fieldValue is XElement xmlElement))
			{
				throw new XmlException("The serialized result of member {0} of type {1} did not return a valid {2} result.", memberInfo.Name, memberInfo.DeclaringType?.Name, nameof(XElement));
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

			ISerializationReflectionMap typeMap = SerializationUtilities.GetTypeMap(target.GetType());
			ISerializableMember[] members = typeMap.GetSerializableMembers(typeof(AbstractXmlMemberAttribute));

			if (XmlDefinition.ParallelProcessingEnabled && (members.Length > 1))
			{
				object parallelLock = new object();
				Parallel.ForEach(members, member =>
				{
					object result = DeserializeMember(member);
					lock (parallelLock) member.SetValue(target, result);
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

			object DeserializeMember(ISerializableMember member)
			{
				object processedResult = member.Attribute switch
				{
					XmlAttributeAttribute attributeAttribute => Deserialize(source, member, attributeAttribute, typeMap),
					XmlElementAttribute elementAttribute => Deserialize(source, member, elementAttribute, typeMap),
					XmlListElementAttribute listElementAttribute => Deserialize(source, member, listElementAttribute, typeMap),
					XmlCDataAttribute cdataElementAttribute => Deserialize(source, member, cdataElementAttribute, typeMap),
					_ => throw new XmlException("Unsupported XML deserialization attribute of type {0}.", member.Attribute.GetType().Name)
				};

				if ((processedResult == null) && SupportsRequiredValues &&
				    typeMap.TryGetRequiredMemberInfo(member.Member, RequiredValueFeature.RequiredValueAttribute, out IRequiredSerializableMember requiredAttr) &&
				    requiredAttr.RequiredParameterAttribute.NullCheck)
				{
					throw new XmlException("The member '{0}' is marked as required on type {1} but the value is null in the source.", member.Member.Name, member.Member.DeclaringType?.Name);
				}

				return processedResult;
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
				throw new XmlException("The member '{0}' is marked as required on type {1} but is not present in the source.", memberInfo.Member.Name, memberInfo.Member.DeclaringType?.Name);
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
				throw new XmlException("The member '{0}' is marked as required on type {1} but is not present in the source.", memberInfo.Member.Name, memberInfo.Member.DeclaringType?.Name);
			}

			return SerializationUtilities.GetDefaultValue(memberInfo.MemberType);
		}

		private object Deserialize(XElement source, ISerializableMember memberInfo, XmlListElementAttribute listElementInfo, ISerializationReflectionMap typeMap)
		{
			if (!typeof(IList).IsAssignableFrom(memberInfo.MemberType))
			{
				throw new XmlException("Member {0} of type {1} is marked as an XML List, but does not implement any {2} interface to receive these values.", memberInfo.Member.Name, memberInfo.Member.DeclaringType?.Name, nameof(IList));
			}

			XElement childElement = source.Element(GetElementKey(listElementInfo, memberInfo.Member));
			if (childElement != null)
			{
				return Serializer.Deserialize(memberInfo.MemberType, childElement, Definition);
			}

			if (SupportsRequiredValues && typeMap.IsMemberRequired(memberInfo.Member, RequiredValueFeature.RequiredValueAttribute))
			{
				throw new XmlException("The member '{0}' is marked as required on type {1} but is not present in the source.", memberInfo.Member.Name, memberInfo.Member.DeclaringType?.Name);
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
					throw new XmlException("The member '{0}' is marked as required on type {1} but is not present in the source.", memberInfo.Member.Name, memberInfo.Member.DeclaringType?.Name);
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
			ITypeResolutionParameter[] typeResolveAttributes = SerializationUtilities.GetTypeMap(sourceType).GetTypeResolveParameters(typeof(XmlTypeAttribute));

			// Find the attribute with the right target.
			return typeResolveAttributes.Where(tr => tr.Target == sourceType).Cast<XmlTypeAttribute>().FirstOrDefault();
		}
	}
}