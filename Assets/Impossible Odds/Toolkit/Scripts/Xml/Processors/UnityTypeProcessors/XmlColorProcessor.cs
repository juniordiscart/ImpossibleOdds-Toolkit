namespace ImpossibleOdds.Xml.Processors
{
	using System.Xml.Linq;
	using UnityEngine;

	public class XmlColorAttributesProcessor : UnityPrimitiveXmlAttributesProcessor<Color>
	{
		public XmlColorAttributesProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Color value)
		{
			XElement xmlElement = new XElement(string.Empty);
			xmlElement.SetAttributeValue("r", value.r);
			xmlElement.SetAttributeValue("g", value.g);
			xmlElement.SetAttributeValue("b", value.b);
			xmlElement.SetAttributeValue("a", value.a);
			return xmlElement;
		}

		protected override Color Deserialize(XElement xmlData)
		{
			return new Color(
				float.Parse(xmlData.Attribute("r").Value),
				float.Parse(xmlData.Attribute("g").Value),
				float.Parse(xmlData.Attribute("b").Value),
				float.Parse(xmlData.Attribute("a").Value)
			);
		}
	}

	public class XmlColorElementsProcessor : UnityPrimitiveXmlElementsProcessor<Color>
	{
		public XmlColorElementsProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Color value)
		{
			XElement xElement = new XElement(string.Empty);
			xElement.Add(new XElement("r", value.r));
			xElement.Add(new XElement("g", value.g));
			xElement.Add(new XElement("b", value.b));
			xElement.Add(new XElement("a", value.a));

			return xElement;
		}

		protected override Color Deserialize(XElement xmlData)
		{
			return new Color(
				float.Parse(xmlData.Element("r").Value),
				float.Parse(xmlData.Element("g").Value),
				float.Parse(xmlData.Element("b").Value),
				float.Parse(xmlData.Element("a").Value)
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
