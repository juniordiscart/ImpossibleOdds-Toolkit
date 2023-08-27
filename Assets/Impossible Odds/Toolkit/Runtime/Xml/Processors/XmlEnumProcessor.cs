namespace ImpossibleOdds.Xml.Processors
{
	using System;
	using System.Xml.Linq;
	using ImpossibleOdds.Serialization.Processors;

	public class XmlEnumProcessor : EnumProcessor
	{
		public XmlEnumProcessor(XmlSerializationDefinition definition)
		: base(definition)
		{ }

		/// <inheritdoc />
		public override object Deserialize(Type targetType, object dataToDeserialize)
		{
			// If the provided value is an XElement, then extract its value to be processed to an enum value.
			if (dataToDeserialize is XElement xElement)
			{
				dataToDeserialize = xElement.Value;
			}

			return base.Deserialize(targetType, dataToDeserialize);
		}

		/// <inheritdoc />
		public override bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			// If the provided value is an XElement, then extract its value to be processed to an enum value.
			if (dataToDeserialize is XElement xElement)
			{
				dataToDeserialize = xElement.Value;
			}

			return base.CanDeserialize(targetType, dataToDeserialize);
		}
	}
}
