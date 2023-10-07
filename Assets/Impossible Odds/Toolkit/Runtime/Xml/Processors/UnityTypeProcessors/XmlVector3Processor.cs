﻿using System.Xml.Linq;
using UnityEngine;

namespace ImpossibleOdds.Xml.Processors
{
	public class XmlVector3AttributesProcessor : UnityPrimitiveXmlAttributesProcessor<Vector3>
	{
		private const string X = "x";
		private const string Y = "y";
		private const string Z = "z";

		public XmlVector3AttributesProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Vector3 value)
		{
			XElement xmlElement = new XElement(string.Empty);
			xmlElement.SetAttributeValue(X, value.x);
			xmlElement.SetAttributeValue(Y, value.y);
			xmlElement.SetAttributeValue(Z, value.z);
			return xmlElement;
		}

		protected override Vector3 Deserialize(XElement xmlData)
		{
			return new Vector3(
				float.Parse(xmlData.Attribute(X).Value),
				float.Parse(xmlData.Attribute(Y).Value),
				float.Parse(xmlData.Attribute(Z).Value)
			);
		}

		protected override bool CanSerialize(Vector3 primitive)
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

	public class XmlVector3ElementsProcessor : UnityPrimitiveXmlElementsProcessor<Vector3>
	{
		private const string X = "x";
		private const string Y = "y";
		private const string Z = "z";

		public XmlVector3ElementsProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		protected override XElement Serialize(Vector3 value)
		{
			XElement xElement = new XElement(string.Empty);
			xElement.Add(new XElement(X, value.x));
			xElement.Add(new XElement(Y, value.y));
			xElement.Add(new XElement(Z, value.z));

			return xElement;
		}

		protected override Vector3 Deserialize(XElement xmlData)
		{
			return new Vector3(
				float.Parse(xmlData.Element(X).Value),
				float.Parse(xmlData.Element(Y).Value),
				float.Parse(xmlData.Element(Z).Value)
			);
		}

		protected override bool CanSerialize(Vector3 primitive)
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