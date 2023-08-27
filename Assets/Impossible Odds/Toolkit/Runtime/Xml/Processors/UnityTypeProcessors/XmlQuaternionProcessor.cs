namespace ImpossibleOdds.Xml.Processors
{
	using System.Xml.Linq;
	using UnityEngine;

	public class XmlQuaternionAttributesProcessor : UnityPrimitiveXmlAttributesProcessor<Quaternion>
	{
		private const string X = "x";
		private const string Y = "y";
		private const string Z = "z";
		private const string W = "w";

		public XmlQuaternionAttributesProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Quaternion value)
		{
			XElement xmlElement = new XElement(string.Empty);
			xmlElement.SetAttributeValue(X, value.x);
			xmlElement.SetAttributeValue(Y, value.y);
			xmlElement.SetAttributeValue(Z, value.z);
			xmlElement.SetAttributeValue(W, value.w);
			return xmlElement;
		}

		protected override Quaternion Deserialize(XElement xmlData)
		{
			return new Quaternion(
				float.Parse(xmlData.Attribute(X).Value),
				float.Parse(xmlData.Attribute(Y).Value),
				float.Parse(xmlData.Attribute(Z).Value),
				float.Parse(xmlData.Attribute(W).Value)
			);
		}

		protected override bool CanSerialize(Quaternion primitive)
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

	public class XmlQuaternionElementsProcessor : UnityPrimitiveXmlElementsProcessor<Quaternion>
	{
		private const string X = "x";
		private const string Y = "y";
		private const string Z = "z";
		private const string W = "w";

		public XmlQuaternionElementsProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Quaternion value)
		{
			XElement xElement = new XElement(string.Empty);
			xElement.Add(new XElement(X, value.x));
			xElement.Add(new XElement(Y, value.y));
			xElement.Add(new XElement(Z, value.z));
			xElement.Add(new XElement(W, value.w));

			return xElement;
		}

		protected override Quaternion Deserialize(XElement xmlData)
		{
			return new Quaternion(
				float.Parse(xmlData.Element(X).Value),
				float.Parse(xmlData.Element(Y).Value),
				float.Parse(xmlData.Element(Z).Value),
				float.Parse(xmlData.Element(W).Value)
			);
		}

		protected override bool CanSerialize(Quaternion primitive)
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
