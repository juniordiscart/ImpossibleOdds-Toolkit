using System.Xml.Linq;
using UnityEngine;

namespace ImpossibleOdds.Xml.Processors
{
	public class XmlQuaternionAttributesProcessor : UnityPrimitiveXmlAttributesProcessor<Quaternion>
	{
		private static readonly string[] XYZW = {"x", "y", "z", "w"};

		public XmlQuaternionAttributesProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		public override string[] Keys => XYZW;

		protected override XElement Serialize(Quaternion value)
		{
			XElement xmlElement = new XElement(string.Empty);
			xmlElement.SetAttributeValue(XYZW[0], value.x);
			xmlElement.SetAttributeValue(XYZW[1], value.y);
			xmlElement.SetAttributeValue(XYZW[2], value.z);
			xmlElement.SetAttributeValue(XYZW[3], value.w);
			return xmlElement;
		}

		protected override Quaternion Deserialize(XElement xmlData)
		{
			return new Quaternion(
				float.Parse(xmlData.Attribute(XYZW[0]).Value),
				float.Parse(xmlData.Attribute(XYZW[1]).Value),
				float.Parse(xmlData.Attribute(XYZW[2]).Value),
				float.Parse(xmlData.Attribute(XYZW[3]).Value)
			);
		}
	}

	public class XmlQuaternionElementsProcessor : UnityPrimitiveXmlElementsProcessor<Quaternion>
	{
		private static readonly string[] XYZW = {"x", "y", "z", "w"};

		public XmlQuaternionElementsProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		public override string[] Keys => XYZW;

		protected override XElement Serialize(Quaternion value)
		{
			XElement xElement = new XElement(string.Empty);
			xElement.Add(new XElement(XYZW[0], value.x));
			xElement.Add(new XElement(XYZW[1], value.y));
			xElement.Add(new XElement(XYZW[2], value.z));
			xElement.Add(new XElement(XYZW[3], value.w));

			return xElement;
		}

		protected override Quaternion Deserialize(XElement xmlData)
		{
			return new Quaternion(
				float.Parse(xmlData.Element(XYZW[0]).Value),
				float.Parse(xmlData.Element(XYZW[1]).Value),
				float.Parse(xmlData.Element(XYZW[2]).Value),
				float.Parse(xmlData.Element(XYZW[3]).Value)
			);
		}
	}

	public class XmlQuaternionProcessor : UnityPrimitiveXmlSwitchProcessor<XmlQuaternionAttributesProcessor, XmlQuaternionElementsProcessor, Quaternion>
	{
		public XmlQuaternionProcessor(XmlSerializationDefinition definition, XmlPrimitiveProcessingMethod preferredProcessingMethod)
		: this(new XmlQuaternionAttributesProcessor(definition), new XmlQuaternionElementsProcessor(definition), preferredProcessingMethod)
		{ }

		public XmlQuaternionProcessor(XmlQuaternionAttributesProcessor attributesProcessor, XmlQuaternionElementsProcessor elementsProcessor, XmlPrimitiveProcessingMethod preferredProcessingMethod)
		: base(attributesProcessor, elementsProcessor, preferredProcessingMethod)
		{ }
	}
}