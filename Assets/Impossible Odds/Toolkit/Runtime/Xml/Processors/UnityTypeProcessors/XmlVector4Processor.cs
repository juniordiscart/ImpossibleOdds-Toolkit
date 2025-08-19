using System.Xml.Linq;
using UnityEngine;

namespace ImpossibleOdds.Xml.Processors
{
	public class XmlVector4AttributesProcessor : UnityPrimitiveXmlAttributesProcessor<Vector4>
	{
		private static readonly string[] XYZW = {"x", "y", "z", "w"};

		public XmlVector4AttributesProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		public override string[] Keys => XYZW;

		protected override XElement Serialize(Vector4 value)
		{
			XElement xmlElement = new XElement(string.Empty);
			xmlElement.SetAttributeValue(XYZW[0], value.x);
			xmlElement.SetAttributeValue(XYZW[1], value.y);
			xmlElement.SetAttributeValue(XYZW[2], value.z);
			xmlElement.SetAttributeValue(XYZW[3], value.w);
			return xmlElement;
		}

		protected override Vector4 Deserialize(XElement xmlData)
		{
			return new Vector4(
				float.Parse(xmlData.Attribute(XYZW[0]).Value),
				float.Parse(xmlData.Attribute(XYZW[1]).Value),
				float.Parse(xmlData.Attribute(XYZW[2]).Value),
				float.Parse(xmlData.Attribute(XYZW[3]).Value)
			);
		}
	}

	public class XmlVector4ElementsProcessor : UnityPrimitiveXmlElementsProcessor<Vector4>
	{
		private static readonly string[] XYZW = {"x", "y", "z", "w"};

		public XmlVector4ElementsProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		public override string[] Keys => XYZW;

		protected override XElement Serialize(Vector4 value)
		{
			XElement xElement = new XElement(string.Empty);
			xElement.Add(new XElement(XYZW[0], value.x));
			xElement.Add(new XElement(XYZW[1], value.y));
			xElement.Add(new XElement(XYZW[2], value.z));
			xElement.Add(new XElement(XYZW[3], value.w));

			return xElement;
		}

		protected override Vector4 Deserialize(XElement xmlData)
		{
			return new Vector4(
				float.Parse(xmlData.Element(XYZW[0]).Value),
				float.Parse(xmlData.Element(XYZW[1]).Value),
				float.Parse(xmlData.Element(XYZW[2]).Value),
				float.Parse(xmlData.Element(XYZW[3]).Value)
			);
		}
	}

	public class XmlVector4Processor : UnityPrimitiveXmlSwitchProcessor<XmlVector4AttributesProcessor, XmlVector4ElementsProcessor, Vector4>
	{
		public XmlVector4Processor(XmlSerializationDefinition definition, XmlPrimitiveProcessingMethod preferredProcessingMethod)
		: this(new XmlVector4AttributesProcessor(definition), new XmlVector4ElementsProcessor(definition), preferredProcessingMethod)
		{ }

		public XmlVector4Processor(XmlVector4AttributesProcessor attributesProcessor, XmlVector4ElementsProcessor elementsProcessor, XmlPrimitiveProcessingMethod preferredProcessingMethod)
		: base(attributesProcessor, elementsProcessor, preferredProcessingMethod)
		{ }
	}
}