namespace ImpossibleOdds.Xml.Processors
{
	using System.Xml.Linq;
	using UnityEngine;

	public class XmlVector3AttributesProcessor : UnityPrimitiveXmlAttributesProcessor<Vector3>
	{
		public XmlVector3AttributesProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Vector3 value)
		{
			XElement xmlElement = new XElement(string.Empty);
			xmlElement.SetAttributeValue("x", value.x);
			xmlElement.SetAttributeValue("y", value.y);
			xmlElement.SetAttributeValue("z", value.z);
			return xmlElement;
		}

		protected override Vector3 Deserialize(XElement xmlData)
		{
			return new Vector3(
				float.Parse(xmlData.Attribute("x").Value),
				float.Parse(xmlData.Attribute("y").Value),
				float.Parse(xmlData.Attribute("z").Value)
			);
		}
	}

	public class XmlVector3ElementsProcessor : UnityPrimitiveXmlElementsProcessor<Vector3>
	{
		public XmlVector3ElementsProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Vector3 value)
		{
			XElement xElement = new XElement(string.Empty);
			xElement.Add(new XElement("x", value.x));
			xElement.Add(new XElement("y", value.y));
			xElement.Add(new XElement("z", value.z));

			return xElement;
		}

		protected override Vector3 Deserialize(XElement xmlData)
		{
			return new Vector3(
				float.Parse(xmlData.Element("x").Value),
				float.Parse(xmlData.Element("y").Value),
				float.Parse(xmlData.Element("z").Value)
			);
		}
	}

	public class XmlVector3Processor : UnityPrimitiveXmlSwitchProcessor<XmlVector3AttributesProcessor, XmlVector3ElementsProcessor, Vector3>
	{
		public XmlVector3Processor(XmlSerializationDefinition definition, XmlPrimitiveProcessingMethod preferredProcessingMethod)
		: this(new XmlVector3AttributesProcessor(definition), new XmlVector3ElementsProcessor(definition), preferredProcessingMethod)
		{ }

		public XmlVector3Processor(XmlVector3AttributesProcessor attributesProcessor, XmlVector3ElementsProcessor elementsProcessor, XmlPrimitiveProcessingMethod preferredProcessingMethod)
		: base(attributesProcessor, elementsProcessor, preferredProcessingMethod)
		{ }
	}
}
