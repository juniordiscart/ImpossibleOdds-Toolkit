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
    public class XmlCustomSequenceProcessor : AbstractCustomObjectProcessor, IDeserializationToTargetProcessor
    {
        private readonly XmlSerializationDefinition xmlDefinition;
        private readonly XmlSequenceProcessor sequenceProcessor;

        public XmlSerializationDefinition XmlDefinition => xmlDefinition;

        public XmlCustomSequenceProcessor(XmlSerializationDefinition definition, XmlSequenceProcessor sequenceProcessor) : base(definition)
        {
            definition.ThrowIfNull(nameof(definition));
            sequenceProcessor.ThrowIfNull(nameof(sequenceProcessor));
            
            xmlDefinition = definition;
            this.sequenceProcessor = sequenceProcessor;
        }

        public override bool Serialize(object objectToSerialize, out object serializedResult)
        {
            if (objectToSerialize == null)
            {
                serializedResult = objectToSerialize;
                return true;
            }
            
            Type sourceType = objectToSerialize.GetType();
            if (!Attribute.IsDefined(sourceType, typeof(XmlListAttribute), true))
            {
                serializedResult = null;
                return false;
            }

            InvokeOnSerializationCallback(objectToSerialize);

            if (!sequenceProcessor.Serialize(objectToSerialize, out serializedResult))
            {
                throw new XmlException($"The object of type {sourceType.Name} failed to be serialized by the {nameof(XmlSequenceProcessor)}.");
            }
            
            Serialize(sourceType, objectToSerialize, (XElement)serializedResult);
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
            
            if (!(dataToDeserialize is XElement))
            {
                deserializedResult = null;
                return false;
            }

            Type instanceType = ResolveTypeForDeserialization(targetType, dataToDeserialize as XElement);
            if (!Attribute.IsDefined(instanceType, typeof(XmlListAttribute), true))
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
            
            deserializedResult = null;
            return false;
        }

        public bool Deserialize(object deserializationTarget, object dataToDeserialize)
        {
            deserializationTarget.ThrowIfNull(nameof(deserializationTarget));
            
            // If the source value is null, then there is little to do.
            if (dataToDeserialize == null)
            {
                return true;
            }

            Type targetType = deserializationTarget.GetType();
            if (!Attribute.IsDefined(targetType, typeof(XmlListAttribute), true))
            {
                return false;
            }

            InvokeOnDeserializationCallback(deserializationTarget);
            Deserialize(deserializationTarget, (XElement)dataToDeserialize);

            if (!sequenceProcessor.Deserialize(deserializationTarget, dataToDeserialize))
            {
                throw new XmlException($"The {nameof(XmlSequenceProcessor)} failed to deserialize to an instance of type {targetType.Name}.");
            }
            
            InvokeOnDeserializedCallback(deserializationTarget);
            return true;
        }

        private void Serialize(Type sourceType, object source, XElement element)
        {
            ISerializationReflectionMap sourceTypeCache = SerializationUtilities.GetTypeMap(sourceType);
            ISerializableMember[] elementFields = sourceTypeCache.GetSerializableMembers(typeof(AbstractXmlMemberAttribute));
            object parallelLock = null;

            if (XmlDefinition.ParallelProcessingEnabled && (elementFields.Length > 1))
            {
                parallelLock = new object();
                Parallel.ForEach(elementFields, SerializeMember);
            }
            else
            {
                Array.ForEach(elementFields, SerializeMember);
            }

            // Add the type information, if any.
            XmlTypeAttribute typeResolveParameter = ResolveTypeForSerialization(sourceType);
            if (typeResolveParameter == null)
            {
                return;
            }

            // If the type key already exists either as an attribute, then quit.
            XName typeKey = typeResolveParameter.KeyOverride ?? xmlDefinition.TypeResolveKey;
            if (element.Attribute(typeKey) != null)
            {
                return;
            }

            object typeValue = typeResolveParameter.Value ?? typeResolveParameter.Target.Name;
            typeValue = Serializer.Serialize(typeValue, XmlDefinition.AttributeSerializationDefinition);
            typeValue = SerializationUtilities.PostProcessValue<string>(typeValue);
            element.SetAttributeValue(typeKey, typeValue);

            return;

            // Only XML attributes are supported in this processor
            void SerializeMember(ISerializableMember sourceMember)
            {
                object value = sourceMember.GetValue(source);

                XObject xmlResult = null;
                if (sourceMember.Attribute is XmlAttributeAttribute attributeAttribute)
                {
                    xmlResult = Serialize(value, sourceMember.Member, attributeAttribute);
                }
                else
                {
                    throw new XmlException($"Unsupported XML serialization attribute of type {sourceMember.Attribute.GetType().Name} for use with {nameof(XmlListAttribute)}.");
                }

                if (parallelLock != null)
                {
                    lock (parallelLock) element.Add(xmlResult);
                }
                else
                {
                    element.Add(xmlResult);
                }
            }
        }
        
        private XAttribute Serialize(object fieldValue, MemberInfo memberInfo, XmlAttributeAttribute attributeAttribute)
        {
            return new XAttribute(GetElementKey(attributeAttribute, memberInfo), Serializer.Serialize(fieldValue, XmlDefinition.AttributeSerializationDefinition));
        }
        
        private string GetElementKey(AbstractXmlMemberAttribute xmlAttribute, MemberInfo member)
        {
            return !string.IsNullOrWhiteSpace(xmlAttribute.Key) ? xmlAttribute.Key : member.Name;
        }
        
        private XmlTypeAttribute ResolveTypeForSerialization(Type sourceType)
        {
            ITypeResolveParameter[] typeResolveAttributes = SerializationUtilities.GetTypeMap(sourceType).GetTypeResolveParameters(typeof(XmlTypeAttribute));

            // Find the attribute with the right target.
            foreach (ITypeResolveParameter tr in typeResolveAttributes)
            {
                if (tr.Target == sourceType)
                {
                    return tr as XmlTypeAttribute;
                }
            }

            return null;
        }

        private void Deserialize(object target, XElement source)
        {
            // Process attributes. Don't bother if it doesn't have any.
			if (!source.HasAttributes)
			{
				return;
			}

			ISerializationReflectionMap typeMap = SerializationUtilities.GetTypeMap(target.GetType());
			ISerializableMember[] members = typeMap.GetSerializableMembers(typeof(AbstractXmlMemberAttribute));
			object parallelLock = null;

			if (XmlDefinition.ParallelProcessingEnabled && (members.Length > 1))
			{
				parallelLock = new object();
				Parallel.ForEach(members, DeserializeMember);
			}
			else
			{
				Array.ForEach(members, DeserializeMember);
			}

			void DeserializeMember(ISerializableMember member)
			{
				object processedResult = null;

				if (member.Attribute is XmlAttributeAttribute attributeAttribute)
				{
					processedResult = Deserialize(source, member, attributeAttribute, typeMap);
				}
				else
				{
					throw new XmlException($"Unsupported XML deserialization attribute of type {member.Attribute.GetType().Name}.");
				}

				if ((processedResult == null) && typeMap.TryGetRequiredMemberInfo(member.Member, xmlDefinition.RequiredAttributeType, out IRequiredSerializableMember requiredAttr) && requiredAttr.RequiredParameterAttribute.NullCheck)
				{
					throw new XmlException($"The member '{member.Member.Name}' is marked as required on type {member.Member.DeclaringType.Name} but the value is null in the source.");
				}

				if (parallelLock != null)
				{
					lock (parallelLock) member.SetValue(target, processedResult);
				}
				else
				{
					member.SetValue(target, processedResult);
				}
			}
        }
		
		private object Deserialize(XElement source, ISerializableMember memberInfo, XmlAttributeAttribute attributeInfo, ISerializationReflectionMap typeMap)
		{
			XAttribute attribute = source.Attribute(GetElementKey(attributeInfo, memberInfo.Member));
			if (attribute != null)
			{
				return Serializer.Deserialize(memberInfo.MemberType, attribute.Value, XmlDefinition.AttributeSerializationDefinition);
			}
			
			if (typeMap.IsMemberRequired(memberInfo.Member, xmlDefinition.RequiredAttributeType))
			{
				throw new XmlException($"The member '{memberInfo.Member.Name}' is marked as required on type {memberInfo.Member.DeclaringType.Name} but is not present in the source.");
			}
			
			return SerializationUtilities.GetDefaultValue(memberInfo.MemberType);
		}
        
        private Type ResolveTypeForDeserialization(Type targetType, XElement element)
		{
			ITypeResolveParameter[] typeResolveAttrs =
				SerializationUtilities.GetTypeMap(targetType).
				GetTypeResolveParameters(typeof(XmlTypeAttribute));

			foreach (XmlTypeAttribute typeResolveAttr in typeResolveAttrs.Cast<XmlTypeAttribute>())
			{
				// Perform this filter as well as a way to stop the recursion.
				if (typeResolveAttr.Target == targetType)
				{
					continue;
				}

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
       
                    throw new SerializationException($"The attribute of type {typeResolveAttr.GetType().Name}, defined on type {targetType.Name} or its super types, is matched but cannot be assigned from instance of type {typeResolveAttr.Target.Name}.");
                }
			}

			return targetType;
		}

		private Type ResolveTypeForDeserialization(XElement element, XmlTypeAttribute typeResolveParam, XName processedKey)
		{
			if ((processedKey == null) || !element.HasAttributes)
			{
				return null;
			}
            
            XAttribute xmlTypeAttr = element.Attribute(processedKey);
            if (xmlTypeAttr == null)
            {
                return null;
            }
            
            object value = typeResolveParam.Value ?? typeResolveParam.Target.Name;
            string processedValue = SerializationUtilities.PostProcessValue<string>(Serializer.Serialize(value, Definition));
            return xmlTypeAttr.Value.Equals(processedValue) ? typeResolveParam.Target : null;
        }
    }
}
