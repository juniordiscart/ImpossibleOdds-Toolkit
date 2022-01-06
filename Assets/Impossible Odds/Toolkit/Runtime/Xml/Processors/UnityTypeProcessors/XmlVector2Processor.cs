namespace ImpossibleOdds.Xml.Processors
{
	using System.Xml.Linq;
	using UnityEngine;

	public class XmlVector2AttributesProcessor : UnityPrimitiveXmlAttributesProcessor<Vector2>
	{
		public XmlVector2AttributesProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Vector2 value)
		{
			XElement xmlElement = new XElement(string.Empty);
			xmlElement.SetAttributeValue("x", value.x);
			xmlElement.SetAttributeValue("y", value.y);
			return xmlElement;
		}

		protected override Vector2 Deserialize(XElement xmlData)
		{
			return new Vector2(
				float.Parse(xmlData.Attribute("x").Value),
				float.Parse(xmlData.Attribute("y").Value)
			);
		}
	}

	public class XmlVector2ElementsProcessor : UnityPrimitiveXmlElementsProcessor<Vector2>
	{
		public XmlVector2ElementsProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Vector2 value)
		{
			XElement xElement = new XElement(string.Empty);
			xElement.Add(new XElement("x", value.x));
			xElement.Add(new XElement("y", value.y));

			return xElement;
		}

		protected override Vector2 Deserialize(XElement xmlData)
		{
			return new Vector2(
				float.Parse(xmlData.Element("x").Value),
				float.Parse(xmlData.Element("y").Value)
			);
		}
	}

	public class XmlVector2Processor : UnityPrimitiveXmlSwitchProcessor<XmlVector2AttributesProcessor, XmlVector2ElementsProcessor, Vector2>
	{
		public XmlVector2Processor(XmlSerializationDefinition definition, XmlPrimitiveProcessingMethod preferredProcessingMethod)
		: this(new XmlVector2AttributesProcessor(definition), new XmlVector2ElementsProcessor(definition), preferredProcessingMethod)
		{ }

		public XmlVector2Processor(XmlVector2AttributesProcessor attributesProcessor, XmlVector2ElementsProcessor elementsProcessor, XmlPrimitiveProcessingMethod preferredProcessingMethod)
		: base(attributesProcessor, elementsProcessor, preferredProcessingMethod)
		{ }
	}
}
