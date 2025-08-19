using System.Xml.Linq;
using UnityEngine;

namespace ImpossibleOdds.Xml.Processors
{
	public class XmlVector2IntAttributesProcessor : UnityPrimitiveXmlAttributesProcessor<Vector2Int>
	{
		private static readonly string[] XY = {"x", "y"};

		public XmlVector2IntAttributesProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		public override string[] Keys => XY;

		protected override XElement Serialize(Vector2Int value)
		{
			XElement xmlElement = new XElement(string.Empty);
			xmlElement.SetAttributeValue(XY[0], value.x);
			xmlElement.SetAttributeValue(XY[1], value.y);
			return xmlElement;
		}

		protected override Vector2Int Deserialize(XElement xmlData)
		{
			return new Vector2Int(
				int.Parse(xmlData.Attribute(XY[0]).Value),
				int.Parse(xmlData.Attribute(XY[1]).Value)
			);
		}
	}

	public class XmlVector2IntElementsProcessor : UnityPrimitiveXmlElementsProcessor<Vector2Int>
	{
		private static readonly string[] XY = {"x", "y"};

		public XmlVector2IntElementsProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		public override string[] Keys => XY;

		protected override XElement Serialize(Vector2Int value)
		{
			XElement xElement = new XElement(string.Empty);
			xElement.Add(new XElement(XY[0], value.x));
			xElement.Add(new XElement(XY[1], value.y));

			return xElement;
		}

		protected override Vector2Int Deserialize(XElement xmlData)
		{
			return new Vector2Int(
				int.Parse(xmlData.Element(XY[0]).Value),
				int.Parse(xmlData.Element(XY[1]).Value)
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