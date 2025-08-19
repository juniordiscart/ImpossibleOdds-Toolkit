using System.Xml.Linq;
using UnityEngine;

namespace ImpossibleOdds.Xml.Processors
{
	public class XmlVector2AttributesProcessor : UnityPrimitiveXmlAttributesProcessor<Vector2>
	{
		private static readonly string[] XY = {"x", "y"};

		public XmlVector2AttributesProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		public override string[] Keys => XY;

		protected override XElement Serialize(Vector2 value)
		{
			XElement xmlElement = new XElement(string.Empty);
			xmlElement.SetAttributeValue(XY[0], value.x);
			xmlElement.SetAttributeValue(XY[1], value.y);
			return xmlElement;
		}

		protected override Vector2 Deserialize(XElement xmlData)
		{
			return new Vector2(
				float.Parse(xmlData.Attribute(XY[0]).Value),
				float.Parse(xmlData.Attribute(XY[1]).Value)
			);
		}
	}

	public class XmlVector2ElementsProcessor : UnityPrimitiveXmlElementsProcessor<Vector2>
	{
		private static readonly string[] XY = {"x", "y"};

		public XmlVector2ElementsProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		public override string[] Keys => XY;

		protected override XElement Serialize(Vector2 value)
		{
			XElement xElement = new XElement(string.Empty);
			xElement.Add(new XElement(XY[0], value.x));
			xElement.Add(new XElement(XY[1], value.y));

			return xElement;
		}

		protected override Vector2 Deserialize(XElement xmlData)
		{
			return new Vector2(
				float.Parse(xmlData.Element(XY[0]).Value),
				float.Parse(xmlData.Element(XY[1]).Value)
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