using System.Xml.Linq;
using UnityEngine;

namespace ImpossibleOdds.Xml.Processors
{
	public class XmlVector3IntAttributesProcessor : UnityPrimitiveXmlAttributesProcessor<Vector3Int>
	{
		private static readonly string[] XYZ = {"x", "y", "z"};

		public XmlVector3IntAttributesProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		public override string[] Keys => XYZ;

		protected override XElement Serialize(Vector3Int value)
		{
			XElement xmlElement = new XElement(string.Empty);
			xmlElement.SetAttributeValue(XYZ[0], value.x);
			xmlElement.SetAttributeValue(XYZ[1], value.y);
			xmlElement.SetAttributeValue(XYZ[2], value.z);
			return xmlElement;
		}

		protected override Vector3Int Deserialize(XElement xmlData)
		{
			return new Vector3Int(
				int.Parse(xmlData.Attribute(XYZ[0]).Value),
				int.Parse(xmlData.Attribute(XYZ[1]).Value),
				int.Parse(xmlData.Attribute(XYZ[2]).Value)
			);
		}
	}

	public class XmlVector3IntElementsProcessor : UnityPrimitiveXmlElementsProcessor<Vector3Int>
	{
		private static readonly string[] XYZ = {"x", "y", "z"};

		public XmlVector3IntElementsProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		public override string[] Keys => XYZ;

		protected override XElement Serialize(Vector3Int value)
		{
			XElement xElement = new XElement(string.Empty);
			xElement.Add(new XElement(XYZ[0], value.x));
			xElement.Add(new XElement(XYZ[1], value.y));
			xElement.Add(new XElement(XYZ[2], value.z));

			return xElement;
		}

		protected override Vector3Int Deserialize(XElement xmlData)
		{
			return new Vector3Int(
				int.Parse(xmlData.Element(XYZ[0]).Value),
				int.Parse(xmlData.Element(XYZ[1]).Value),
				int.Parse(xmlData.Element(XYZ[2]).Value)
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