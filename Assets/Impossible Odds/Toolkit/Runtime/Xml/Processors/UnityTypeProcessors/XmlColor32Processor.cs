using System.Xml.Linq;
using UnityEngine;

namespace ImpossibleOdds.Xml.Processors
{
	public class XmlColor32AttributesProcessor : UnityPrimitiveXmlAttributesProcessor<Color32>
	{
		private const string R = "r";
		private const string G = "g";
		private const string B = "b";
		private const string A = "a";

		public XmlColor32AttributesProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Color32 value)
		{
			XElement xmlElement = new XElement(string.Empty);
			xmlElement.SetAttributeValue(R, value.r);
			xmlElement.SetAttributeValue(G, value.g);
			xmlElement.SetAttributeValue(B, value.b);
			xmlElement.SetAttributeValue(A, value.a);
			return xmlElement;
		}

		protected override Color32 Deserialize(XElement xmlData)
		{
			return new Color32(
				byte.Parse(xmlData.Attribute(R).Value),
				byte.Parse(xmlData.Attribute(G).Value),
				byte.Parse(xmlData.Attribute(B).Value),
				byte.Parse(xmlData.Attribute(A).Value)
			);
		}

		protected override bool CanSerialize(Color32 primitive)
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

	public class XmlColor32ElementsProcessor : UnityPrimitiveXmlElementsProcessor<Color32>
	{
		private const string R = "r";
		private const string G = "g";
		private const string B = "b";
		private const string A = "a";

		public XmlColor32ElementsProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Color32 value)
		{
			XElement xElement = new XElement(string.Empty);
			xElement.Add(new XElement(R, value.r));
			xElement.Add(new XElement(G, value.g));
			xElement.Add(new XElement(B, value.b));
			xElement.Add(new XElement(A, value.a));

			return xElement;
		}

		protected override Color32 Deserialize(XElement xmlData)
		{
			return new Color32(
				byte.Parse(xmlData.Element(R).Value),
				byte.Parse(xmlData.Element(G).Value),
				byte.Parse(xmlData.Element(B).Value),
				byte.Parse(xmlData.Element(A).Value)
			);
		}

		protected override bool CanSerialize(Color32 primitive)
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