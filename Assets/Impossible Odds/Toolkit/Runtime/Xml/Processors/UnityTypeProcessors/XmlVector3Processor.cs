using System.Xml.Linq;
using UnityEngine;

namespace ImpossibleOdds.Xml.Processors
{
	public class XmlVector3AttributesProcessor : UnityPrimitiveXmlAttributesProcessor<Vector3>
	{
		private static readonly string[] XYZ = {"x", "y", "z"};

		public XmlVector3AttributesProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		public override string[] Keys => XYZ;

		protected override XElement Serialize(Vector3 value)
		{
			XElement xmlElement = new XElement(string.Empty);
			xmlElement.SetAttributeValue(XYZ[0], value.x);
			xmlElement.SetAttributeValue(XYZ[1], value.y);
			xmlElement.SetAttributeValue(XYZ[2], value.z);
			return xmlElement;
		}

		protected override Vector3 Deserialize(XElement xmlData)
		{
			return new Vector3(
				float.Parse(xmlData.Attribute(XYZ[0]).Value),
				float.Parse(xmlData.Attribute(XYZ[1]).Value),
				float.Parse(xmlData.Attribute(XYZ[2]).Value)
			);
		}
	}

	public class XmlVector3ElementsProcessor : UnityPrimitiveXmlElementsProcessor<Vector3>
	{
		private static readonly string[] XYZ = {"x", "y", "z"};

		public XmlVector3ElementsProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		public override string[] Keys => XYZ;

		protected override XElement Serialize(Vector3 value)
		{
			XElement xElement = new XElement(string.Empty);
			xElement.Add(new XElement(XYZ[0], value.x));
			xElement.Add(new XElement(XYZ[1], value.y));
			xElement.Add(new XElement(XYZ[2], value.z));

			return xElement;
		}

		protected override Vector3 Deserialize(XElement xmlData)
		{
			return new Vector3(
				float.Parse(xmlData.Element(XYZ[0]).Value),
				float.Parse(xmlData.Element(XYZ[1]).Value),
				float.Parse(xmlData.Element(XYZ[2]).Value)
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