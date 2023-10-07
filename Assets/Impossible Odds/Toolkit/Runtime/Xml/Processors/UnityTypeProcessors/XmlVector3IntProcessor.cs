using System.Xml.Linq;
using UnityEngine;

namespace ImpossibleOdds.Xml.Processors
{
	public class XmlVector3IntAttributesProcessor : UnityPrimitiveXmlAttributesProcessor<Vector3Int>
	{
		private const string X = "x";
		private const string Y = "y";
		private const string Z = "z";

		public XmlVector3IntAttributesProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Vector3Int value)
		{
			XElement xmlElement = new XElement(string.Empty);
			xmlElement.SetAttributeValue(X, value.x);
			xmlElement.SetAttributeValue(Y, value.y);
			xmlElement.SetAttributeValue(Z, value.z);
			return xmlElement;
		}

		protected override Vector3Int Deserialize(XElement xmlData)
		{
			return new Vector3Int(
				int.Parse(xmlData.Attribute(X).Value),
				int.Parse(xmlData.Attribute(Y).Value),
				int.Parse(xmlData.Attribute(Z).Value)
			);
		}

		protected override bool CanSerialize(Vector3Int primitive)
		{
			return true;
		}

		protected override bool CanDeserialize(XElement element)
		{
			return
				(element.Attribute(X) != null) &&
				(element.Attribute(Y) != null) &&
				(element.Attribute(Z) != null);
		}
	}

	public class XmlVector3IntElementsProcessor : UnityPrimitiveXmlElementsProcessor<Vector3Int>
	{
		private const string X = "x";
		private const string Y = "y";
		private const string Z = "z";

		public XmlVector3IntElementsProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Vector3Int value)
		{
			XElement xElement = new XElement(string.Empty);
			xElement.Add(new XElement(X, value.x));
			xElement.Add(new XElement(Y, value.y));
			xElement.Add(new XElement(Z, value.z));

			return xElement;
		}

		protected override Vector3Int Deserialize(XElement xmlData)
		{
			return new Vector3Int(
				int.Parse(xmlData.Element(X).Value),
				int.Parse(xmlData.Element(Y).Value),
				int.Parse(xmlData.Element(Z).Value)
			);
		}

		protected override bool CanSerialize(Vector3Int primitive)
		{
			return true;
		}

		protected override bool CanDeserialize(XElement element)
		{
			return
				(element.Element(X) != null) &&
				(element.Element(Y) != null) &&
				(element.Element(Z) != null);
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