using System.Xml.Linq;
using UnityEngine;

namespace ImpossibleOdds.Xml.Processors
{
	public class XmlColorAttributesProcessor : UnityPrimitiveXmlAttributesProcessor<Color>
	{
		private const string R = "r";
		private const string G = "g";
		private const string B = "b";
		private const string A = "a";

		public XmlColorAttributesProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Color value)
		{
			XElement xmlElement = new XElement(string.Empty);
			xmlElement.SetAttributeValue(R, value.r);
			xmlElement.SetAttributeValue(G, value.g);
			xmlElement.SetAttributeValue(B, value.b);
			xmlElement.SetAttributeValue(A, value.a);
			return xmlElement;
		}

		protected override Color Deserialize(XElement xmlData)
		{
			return new Color(
				float.Parse(xmlData.Attribute(R).Value),
				float.Parse(xmlData.Attribute(G).Value),
				float.Parse(xmlData.Attribute(B).Value),
				float.Parse(xmlData.Attribute(A).Value)
			);
		}

		protected override bool CanSerialize(Color primitive)
		{
			return true;
		}

		protected override bool CanDeserialize(XElement element)
		{
			return
				(element.Attribute(R) != null) &&
				(element.Attribute(G) != null) &&
				(element.Attribute(B) != null) &&
				(element.Attribute(A) != null);
		}
	}

	public class XmlColorElementsProcessor : UnityPrimitiveXmlElementsProcessor<Color>
	{
		private const string R = "r";
		private const string G = "g";
		private const string B = "b";
		private const string A = "a";

		public XmlColorElementsProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Color value)
		{
			XElement xElement = new XElement(string.Empty);
			xElement.Add(new XElement(R, value.r));
			xElement.Add(new XElement(G, value.g));
			xElement.Add(new XElement(B, value.b));
			xElement.Add(new XElement(A, value.a));

			return xElement;
		}

		protected override Color Deserialize(XElement xmlData)
		{
			return new Color(
				float.Parse(xmlData.Element(R).Value),
				float.Parse(xmlData.Element(G).Value),
				float.Parse(xmlData.Element(B).Value),
				float.Parse(xmlData.Element(A).Value)
			);
		}

		protected override bool CanSerialize(Color primitive)
		{
			return true;
		}

		protected override bool CanDeserialize(XElement element)
		{
			return
				(element.Element(R) != null) &&
				(element.Element(G) != null) &&
				(element.Element(B) != null) &&
				(element.Element(A) != null);
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