using System.Xml.Linq;
using UnityEngine;

namespace ImpossibleOdds.Xml.Processors
{
	public class XmlColor32AttributesProcessor : UnityPrimitiveXmlAttributesProcessor<Color32>
	{
		private static readonly string[] RGBA = {"r", "g", "b", "a"};

		public XmlColor32AttributesProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		public override string[] Keys => RGBA;

		protected override XElement Serialize(Color32 value)
		{
			XElement xmlElement = new XElement(string.Empty);
			xmlElement.SetAttributeValue(RGBA[0], value.r);
			xmlElement.SetAttributeValue(RGBA[1], value.g);
			xmlElement.SetAttributeValue(RGBA[2], value.b);
			xmlElement.SetAttributeValue(RGBA[3], value.a);
			return xmlElement;
		}

		protected override Color32 Deserialize(XElement xmlData)
		{
			return new Color32(
				byte.Parse(xmlData.Attribute(RGBA[0]).Value),
				byte.Parse(xmlData.Attribute(RGBA[1]).Value),
				byte.Parse(xmlData.Attribute(RGBA[2]).Value),
				byte.Parse(xmlData.Attribute(RGBA[3]).Value)
			);
		}
	}

	public class XmlColor32ElementsProcessor : UnityPrimitiveXmlElementsProcessor<Color32>
	{
		private static readonly string[] RGBA = {"r", "g", "b", "a"};
		

		public XmlColor32ElementsProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		public override string[] Keys => RGBA;

		protected override XElement Serialize(Color32 value)
		{
			XElement xElement = new XElement(string.Empty);
			xElement.Add(new XElement(RGBA[0], value.r));
			xElement.Add(new XElement(RGBA[1], value.g));
			xElement.Add(new XElement(RGBA[2], value.b));
			xElement.Add(new XElement(RGBA[3], value.a));

			return xElement;
		}

		protected override Color32 Deserialize(XElement xmlData)
		{
			return new Color32(
				byte.Parse(xmlData.Element(RGBA[0]).Value),
				byte.Parse(xmlData.Element(RGBA[1]).Value),
				byte.Parse(xmlData.Element(RGBA[2]).Value),
				byte.Parse(xmlData.Element(RGBA[3]).Value)
			);
		}
	}

	public class XmlColor32Processor : UnityPrimitiveXmlSwitchProcessor<XmlColor32AttributesProcessor, XmlColor32ElementsProcessor, Color32>
	{
		public XmlColor32Processor(XmlSerializationDefinition definition, XmlPrimitiveProcessingMethod preferredProcessingMethod)
		: this(new XmlColor32AttributesProcessor(definition), new XmlColor32ElementsProcessor(definition), preferredProcessingMethod)
		{ }

		public XmlColor32Processor(XmlColor32AttributesProcessor attributesProcessor, XmlColor32ElementsProcessor elementsProcessor, XmlPrimitiveProcessingMethod preferredProcessingMethod)
		: base(attributesProcessor, elementsProcessor, preferredProcessingMethod)
		{ }
	}
}