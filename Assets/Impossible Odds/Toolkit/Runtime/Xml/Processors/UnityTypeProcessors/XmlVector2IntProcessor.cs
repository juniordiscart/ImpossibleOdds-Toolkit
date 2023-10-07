using System.Xml.Linq;
using UnityEngine;

namespace ImpossibleOdds.Xml.Processors
{
	public class XmlVector2IntAttributesProcessor : UnityPrimitiveXmlAttributesProcessor<Vector2Int>
	{
		private const string X = "x";
		private const string Y = "y";

		public XmlVector2IntAttributesProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Vector2Int value)
		{
			XElement xmlElement = new XElement(string.Empty);
			xmlElement.SetAttributeValue(X, value.x);
			xmlElement.SetAttributeValue(Y, value.y);
			return xmlElement;
		}

		protected override Vector2Int Deserialize(XElement xmlData)
		{
			return new Vector2Int(
				int.Parse(xmlData.Attribute(X).Value),
				int.Parse(xmlData.Attribute(Y).Value)
			);
		}

		protected override bool CanSerialize(Vector2Int primitive)
		{
			return true;
		}

		protected override bool CanDeserialize(XElement element)
		{
			return
				(element.Attribute(X) != null) &&
				(element.Attribute(Y) != null);
		}
	}

	public class XmlVector2IntElementsProcessor : UnityPrimitiveXmlElementsProcessor<Vector2Int>
	{
		private const string X = "x";
		private const string Y = "y";

		public XmlVector2IntElementsProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Vector2Int value)
		{
			XElement xElement = new XElement(string.Empty);
			xElement.Add(new XElement(X, value.x));
			xElement.Add(new XElement(Y, value.y));

			return xElement;
		}

		protected override Vector2Int Deserialize(XElement xmlData)
		{
			return new Vector2Int(
				int.Parse(xmlData.Element(X).Value),
				int.Parse(xmlData.Element(Y).Value)
			);
		}

		protected override bool CanSerialize(Vector2Int primitive)
		{
			return true;
		}

		protected override bool CanDeserialize(XElement element)
		{
			return
				(element.Element(X) != null) &&
				(element.Element(Y) != null);
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