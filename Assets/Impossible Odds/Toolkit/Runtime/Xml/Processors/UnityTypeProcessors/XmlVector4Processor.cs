namespace ImpossibleOdds.Xml.Processors
{
	using System.Xml.Linq;
	using UnityEngine;

	public class XmlVector4AttributesProcessor : UnityPrimitiveXmlAttributesProcessor<Vector4>
	{
		public XmlVector4AttributesProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Vector4 value)
		{
			XElement xmlElement = new XElement(string.Empty);
			xmlElement.SetAttributeValue("x", value.x);
			xmlElement.SetAttributeValue("y", value.y);
			xmlElement.SetAttributeValue("z", value.z);
			xmlElement.SetAttributeValue("w", value.w);
			return xmlElement;
		}

		protected override Vector4 Deserialize(XElement xmlData)
		{
			return new Vector4(
				float.Parse(xmlData.Attribute("x").Value),
				float.Parse(xmlData.Attribute("y").Value),
				float.Parse(xmlData.Attribute("z").Value),
				float.Parse(xmlData.Attribute("w").Value)
			);
		}
	}

	public class XmlVector4ElementsProcessor : UnityPrimitiveXmlElementsProcessor<Vector4>
	{
		public XmlVector4ElementsProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Vector4 value)
		{
			XElement xElement = new XElement(string.Empty);
			xElement.Add(new XElement("x", value.x));
			xElement.Add(new XElement("y", value.y));
			xElement.Add(new XElement("z", value.z));
			xElement.Add(new XElement("w", value.w));

			return xElement;
		}

		protected override Vector4 Deserialize(XElement xmlData)
		{
			return new Vector4(
				float.Parse(xmlData.Element("x").Value),
				float.Parse(xmlData.Element("y").Value),
				float.Parse(xmlData.Element("z").Value),
				float.Parse(xmlData.Element("w").Value)
			);
		}
	}

	public class XmlVector4Processor : UnityPrimitiveXmlSwitchProcessor<XmlVector4AttributesProcessor, XmlVector4ElementsProcessor, Vector4>
	{
		public XmlVector4Processor(XmlSerializationDefinition definition, XmlPrimitiveProcessingMethod preferredProcessingMethod)
		: this(new XmlVector4AttributesProcessor(definition), new XmlVector4ElementsProcessor(definition), preferredProcessingMethod)
		{ }

		public XmlVector4Processor(XmlVector4AttributesProcessor attributesProcessor, XmlVector4ElementsProcessor elementsProcessor, XmlPrimitiveProcessingMethod preferredProcessingMethod)
		: base(attributesProcessor, elementsProcessor, preferredProcessingMethod)
		{ }
	}
}
