namespace ImpossibleOdds.Xml.Processors
{
	using System.Xml.Linq;
	using UnityEngine;

	public class XmlVector2IntAttributesProcessor : UnityPrimitiveXmlAttributesProcessor<Vector2Int>
	{
		public XmlVector2IntAttributesProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Vector2Int value)
		{
			XElement xmlElement = new XElement(string.Empty);
			xmlElement.SetAttributeValue("x", value.x);
			xmlElement.SetAttributeValue("y", value.y);
			return xmlElement;
		}

		protected override Vector2Int Deserialize(XElement xmlData)
		{
			return new Vector2Int(
				int.Parse(xmlData.Attribute("x").Value),
				int.Parse(xmlData.Attribute("y").Value)
			);
		}
	}

	public class XmlVector2IntElementsProcessor : UnityPrimitiveXmlElementsProcessor<Vector2Int>
	{
		public XmlVector2IntElementsProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Vector2Int value)
		{
			XElement xElement = new XElement(string.Empty);
			xElement.Add(new XElement("x", value.x));
			xElement.Add(new XElement("y", value.y));

			return xElement;
		}

		protected override Vector2Int Deserialize(XElement xmlData)
		{
			return new Vector2Int(
				int.Parse(xmlData.Element("x").Value),
				int.Parse(xmlData.Element("y").Value)
			);
		}
	}

	public class XmlVector2IntProcessor : UnityPrimitiveXmlSwitchProcessor<XmlVector2IntAttributesProcessor, XmlVector2IntElementsProcessor, Vector2Int>
	{
		public XmlVector2IntProcessor(XmlSerializationDefinition definition, XmlPrimitiveProcessingMethod preferredProcessingMethod)
		: this(new XmlVector2IntAttributesProcessor(definition), new XmlVector2IntElementsProcessor(definition), preferredProcessingMethod)
		{ }

		public XmlVector2IntProcessor(XmlVector2IntAttributesProcessor attributesProcessor, XmlVector2IntElementsProcessor elementsProcessor, XmlPrimitiveProcessingMethod preferredProcessingMethod)
		: base(attributesProcessor, elementsProcessor, preferredProcessingMethod)
		{ }
	}
}
