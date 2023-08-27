namespace ImpossibleOdds.Xml.Processors
{
	using System.Xml.Linq;
	using UnityEngine;

	public class XmlVector2AttributesProcessor : UnityPrimitiveXmlAttributesProcessor<Vector2>
	{
		private const string X = "x";
		private const string Y = "y";

		public XmlVector2AttributesProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Vector2 value)
		{
			XElement xmlElement = new XElement(string.Empty);
			xmlElement.SetAttributeValue(X, value.x);
			xmlElement.SetAttributeValue(Y, value.y);
			return xmlElement;
		}

		protected override Vector2 Deserialize(XElement xmlData)
		{
			return new Vector2(
				float.Parse(xmlData.Attribute(X).Value),
				float.Parse(xmlData.Attribute(Y).Value)
			);
		}

		protected override bool CanSerialize(Vector2 primitive)
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

	public class XmlVector2ElementsProcessor : UnityPrimitiveXmlElementsProcessor<Vector2>
	{
		private const string X = "x";
		private const string Y = "y";

		public XmlVector2ElementsProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Vector2 value)
		{
			XElement xElement = new XElement(string.Empty);
			xElement.Add(new XElement(X, value.x));
			xElement.Add(new XElement(Y, value.y));

			return xElement;
		}

		protected override Vector2 Deserialize(XElement xmlData)
		{
			return new Vector2(
				float.Parse(xmlData.Element(X).Value),
				float.Parse(xmlData.Element(Y).Value)
			);
		}

		protected override bool CanSerialize(Vector2 primitive)
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
