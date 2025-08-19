using System.Xml.Linq;
using UnityEngine;

namespace ImpossibleOdds.Xml.Processors
{
	public class XmlColorAttributesProcessor : UnityPrimitiveXmlAttributesProcessor<Color>
	{
		private static readonly string[] RGBA = {"r", "g", "b", "a"};

		public XmlColorAttributesProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		public override string[] Keys => RGBA;

		protected override XElement Serialize(Color value)
		{
			XElement xmlElement = new XElement(string.Empty);
			xmlElement.SetAttributeValue(RGBA[0], value.r);
			xmlElement.SetAttributeValue(RGBA[1], value.g);
			xmlElement.SetAttributeValue(RGBA[2], value.b);
			xmlElement.SetAttributeValue(RGBA[3], value.a);
			return xmlElement;
		}

		protected override Color Deserialize(XElement xmlData)
		{
			return new Color(
				float.Parse(xmlData.Attribute(RGBA[0]).Value),
				float.Parse(xmlData.Attribute(RGBA[1]).Value),
				float.Parse(xmlData.Attribute(RGBA[2]).Value),
				float.Parse(xmlData.Attribute(RGBA[3]).Value)
			);
		}
	}

	public class XmlColorElementsProcessor : UnityPrimitiveXmlElementsProcessor<Color>
	{
		private static readonly string[] RGBA = {"r", "g", "b", "a"};

		public XmlColorElementsProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		public override string[] Keys => RGBA;

		protected override XElement Serialize(Color value)
		{
			XElement xElement = new XElement(string.Empty);
			xElement.Add(new XElement(RGBA[0], value.r));
			xElement.Add(new XElement(RGBA[1], value.g));
			xElement.Add(new XElement(RGBA[2], value.b));
			xElement.Add(new XElement(RGBA[3], value.a));

			return xElement;
		}

		protected override Color Deserialize(XElement xmlData)
		{
			return new Color(
				float.Parse(xmlData.Element(RGBA[0]).Value),
				float.Parse(xmlData.Element(RGBA[1]).Value),
				float.Parse(xmlData.Element(RGBA[2]).Value),
				float.Parse(xmlData.Element(RGBA[3]).Value)
			);
		}
	}

	public class XmlColorProcessor : UnityPrimitiveXmlSwitchProcessor<XmlColorAttributesProcessor, XmlColorElementsProcessor, Color>
	{
		public XmlColorProcessor(XmlSerializationDefinition definition, XmlPrimitiveProcessingMethod preferredProcessingMethod)
		: this(new XmlColorAttributesProcessor(definition), new XmlColorElementsProcessor(definition), preferredProcessingMethod)
		{ }

		public XmlColorProcessor(XmlColorAttributesProcessor attributesProcessor, XmlColorElementsProcessor elementsProcessor, XmlPrimitiveProcessingMethod preferredProcessingMethod)
		: base(attributesProcessor, elementsProcessor, preferredProcessingMethod)
		{ }
	}
}