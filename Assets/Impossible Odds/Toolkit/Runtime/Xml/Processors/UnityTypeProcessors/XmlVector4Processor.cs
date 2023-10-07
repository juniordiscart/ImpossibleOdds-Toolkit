using System.Xml.Linq;
using UnityEngine;

namespace ImpossibleOdds.Xml.Processors
{
	public class XmlVector4AttributesProcessor : UnityPrimitiveXmlAttributesProcessor<Vector4>
	{
		private const string X = "x";
		private const string Y = "y";
		private const string Z = "z";
		private const string W = "w";

		public XmlVector4AttributesProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Vector4 value)
		{
			XElement xmlElement = new XElement(string.Empty);
			xmlElement.SetAttributeValue(X, value.x);
			xmlElement.SetAttributeValue(Y, value.y);
			xmlElement.SetAttributeValue(Z, value.z);
			xmlElement.SetAttributeValue(W, value.w);
			return xmlElement;
		}

		protected override Vector4 Deserialize(XElement xmlData)
		{
			return new Vector4(
				float.Parse(xmlData.Attribute(X).Value),
				float.Parse(xmlData.Attribute(Y).Value),
				float.Parse(xmlData.Attribute(Z).Value),
				float.Parse(xmlData.Attribute(W).Value)
			);
		}

		protected override bool CanSerialize(Vector4 primitive)
		{
			return true;
		}

		protected override bool CanDeserialize(XElement element)
		{
			return
				(element.Attribute(X) != null) &&
				(element.Attribute(Y) != null) &&
				(element.Attribute(Z) != null) &&
				(element.Attribute(W) != null);
		}
	}

	public class XmlVector4ElementsProcessor : UnityPrimitiveXmlElementsProcessor<Vector4>
	{
		private const string X = "x";
		private const string Y = "y";
		private const string Z = "z";
		private const string W = "w";

		public XmlVector4ElementsProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Vector4 value)
		{
			XElement xElement = new XElement(string.Empty);
			xElement.Add(new XElement(X, value.x));
			xElement.Add(new XElement(Y, value.y));
			xElement.Add(new XElement(Z, value.z));
			xElement.Add(new XElement(W, value.w));

			return xElement;
		}

		protected override Vector4 Deserialize(XElement xmlData)
		{
			return new Vector4(
				float.Parse(xmlData.Element(X).Value),
				float.Parse(xmlData.Element(Y).Value),
				float.Parse(xmlData.Element(Z).Value),
				float.Parse(xmlData.Element(W).Value)
			);
		}

		protected override bool CanSerialize(Vector4 primitive)
		{
			return true;
		}

		protected override bool CanDeserialize(XElement element)
		{
			return
				(element.Element(X) != null) &&
				(element.Element(Y) != null) &&
				(element.Element(Z) != null) &&
				(element.Element(W) != null);
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