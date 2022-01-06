namespace ImpossibleOdds.Xml.Processors
{
	using System.Xml.Linq;
	using UnityEngine;

	public class XmlQuaternionAttributesProcessor : UnityPrimitiveXmlAttributesProcessor<Quaternion>
	{
		public XmlQuaternionAttributesProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Quaternion value)
		{
			XElement xmlElement = new XElement(string.Empty);
			xmlElement.SetAttributeValue("x", value.x);
			xmlElement.SetAttributeValue("y", value.y);
			xmlElement.SetAttributeValue("z", value.z);
			xmlElement.SetAttributeValue("w", value.w);
			return xmlElement;
		}

		protected override Quaternion Deserialize(XElement xmlData)
		{
			return new Quaternion(
				float.Parse(xmlData.Attribute("x").Value),
				float.Parse(xmlData.Attribute("y").Value),
				float.Parse(xmlData.Attribute("z").Value),
				float.Parse(xmlData.Attribute("w").Value)
			);
		}
	}

	public class XmlQuaternionElementsProcessor : UnityPrimitiveXmlElementsProcessor<Quaternion>
	{
		public XmlQuaternionElementsProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Quaternion value)
		{
			XElement xElement = new XElement(string.Empty);
			xElement.Add(new XElement("x", value.x));
			xElement.Add(new XElement("y", value.y));
			xElement.Add(new XElement("z", value.z));
			xElement.Add(new XElement("w", value.w));

			return xElement;
		}

		protected override Quaternion Deserialize(XElement xmlData)
		{
			return new Quaternion(
				float.Parse(xmlData.Element("x").Value),
				float.Parse(xmlData.Element("y").Value),
				float.Parse(xmlData.Element("z").Value),
				float.Parse(xmlData.Element("w").Value)
			);
		}
	}

	public class XmlQuaternionProcessor : UnityPrimitiveXmlSwitchProcessor<XmlQuaternionAttributesProcessor, XmlQuaternionElementsProcessor, Quaternion>
	{
		public XmlQuaternionProcessor(XmlSerializationDefinition definition, XmlPrimitiveProcessingMethod preferredProcessingMethod)
		: this(new XmlQuaternionAttributesProcessor(definition), new XmlQuaternionElementsProcessor(definition), preferredProcessingMethod)
		{ }

		public XmlQuaternionProcessor(XmlQuaternionAttributesProcessor attributesProcessor, XmlQuaternionElementsProcessor elementsProcessor, XmlPrimitiveProcessingMethod preferredProcessingMethod)
		: base(attributesProcessor, elementsProcessor, preferredProcessingMethod)
		{ }
	}
}
