namespace ImpossibleOdds.Xml.Processors
{
	using System.Xml.Linq;
	using UnityEngine;

	public class XmlColor32AttributesProcessor : UnityPrimitiveXmlAttributesProcessor<Color32>
	{
		public XmlColor32AttributesProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Color32 value)
		{
			XElement xmlElement = new XElement(string.Empty);
			xmlElement.SetAttributeValue("r", value.r);
			xmlElement.SetAttributeValue("g", value.g);
			xmlElement.SetAttributeValue("b", value.b);
			xmlElement.SetAttributeValue("a", value.a);
			return xmlElement;
		}

		protected override Color32 Deserialize(XElement xmlData)
		{
			return new Color32(
				byte.Parse(xmlData.Attribute("r").Value),
				byte.Parse(xmlData.Attribute("g").Value),
				byte.Parse(xmlData.Attribute("b").Value),
				byte.Parse(xmlData.Attribute("a").Value)
			);
		}
	}

	public class XmlColor32ElementsProcessor : UnityPrimitiveXmlElementsProcessor<Color32>
	{
		public XmlColor32ElementsProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Color32 value)
		{
			XElement xElement = new XElement(string.Empty);
			xElement.Add(new XElement("r", value.r));
			xElement.Add(new XElement("g", value.g));
			xElement.Add(new XElement("b", value.b));
			xElement.Add(new XElement("a", value.a));

			return xElement;
		}

		protected override Color32 Deserialize(XElement xmlData)
		{
			return new Color32(
				byte.Parse(xmlData.Element("r").Value),
				byte.Parse(xmlData.Element("g").Value),
				byte.Parse(xmlData.Element("b").Value),
				byte.Parse(xmlData.Element("a").Value)
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
