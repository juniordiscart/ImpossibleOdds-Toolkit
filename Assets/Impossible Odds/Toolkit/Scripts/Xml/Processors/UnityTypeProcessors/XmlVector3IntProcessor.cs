namespace ImpossibleOdds.Xml.Processors
{
	using System.Xml.Linq;
	using UnityEngine;

	public class XmlVector3IntAttributesProcessor : UnityPrimitiveXmlAttributesProcessor<Vector3Int>
	{
		public XmlVector3IntAttributesProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Vector3Int value)
		{
			XElement xmlElement = new XElement(string.Empty);
			xmlElement.SetAttributeValue("x", value.x);
			xmlElement.SetAttributeValue("y", value.y);
			xmlElement.SetAttributeValue("z", value.z);
			return xmlElement;
		}

		protected override Vector3Int Deserialize(XElement xmlData)
		{
			return new Vector3Int(
				int.Parse(xmlData.Attribute("x").Value),
				int.Parse(xmlData.Attribute("y").Value),
				int.Parse(xmlData.Attribute("z").Value)
			);
		}
	}

	public class XmlVector3IntElementsProcessor : UnityPrimitiveXmlElementsProcessor<Vector3Int>
	{
		public XmlVector3IntElementsProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Vector3Int value)
		{
			XElement xElement = new XElement(string.Empty);
			xElement.Add(new XElement("x", value.x));
			xElement.Add(new XElement("y", value.y));
			xElement.Add(new XElement("z", value.z));

			return xElement;
		}

		protected override Vector3Int Deserialize(XElement xmlData)
		{
			return new Vector3Int(
				int.Parse(xmlData.Element("x").Value),
				int.Parse(xmlData.Element("y").Value),
				int.Parse(xmlData.Element("z").Value)
			);
		}
	}

	public class XmlVector3IntProcessor : UnityPrimitiveXmlSwitchProcessor<XmlVector3IntAttributesProcessor, XmlVector3IntElementsProcessor, Vector3Int>
	{
		public XmlVector3IntProcessor(XmlSerializationDefinition definition, XmlPrimitiveProcessingMethod preferredProcessingMethod)
		: this(new XmlVector3IntAttributesProcessor(definition), new XmlVector3IntElementsProcessor(definition), preferredProcessingMethod)
		{ }

		public XmlVector3IntProcessor(XmlVector3IntAttributesProcessor attributesProcessor, XmlVector3IntElementsProcessor elementsProcessor, XmlPrimitiveProcessingMethod preferredProcessingMethod)
		: base(attributesProcessor, elementsProcessor, preferredProcessingMethod)
		{ }
	}
}
