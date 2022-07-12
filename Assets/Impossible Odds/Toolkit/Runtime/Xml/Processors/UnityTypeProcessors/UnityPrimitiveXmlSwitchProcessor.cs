namespace ImpossibleOdds.Xml.Processors
{
	using System;
	using System.Xml.Linq;
	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Serialization.Processors;

	public abstract class UnityPrimitiveXmlSwitchProcessor<TAttributesProcessor, TElementsProcessor, TPrimitive> : IUnityPrimitiveXmlSwitchProcessor, ISerializationProcessor, IDeserializationProcessor
	where TAttributesProcessor : UnityPrimitiveXmlAttributesProcessor<TPrimitive>
	where TElementsProcessor : UnityPrimitiveXmlElementsProcessor<TPrimitive>
	{
		private readonly TAttributesProcessor attributesProcessor = null;
		private readonly TElementsProcessor elementsProcessor = null;
		private XmlPrimitiveProcessingMethod processingMethod;

		public TAttributesProcessor SequenceProcessor
		{
			get => attributesProcessor;
		}

		public TElementsProcessor LookupProcessor
		{
			get => elementsProcessor;
		}

		public XmlPrimitiveProcessingMethod ProcessingMethod
		{
			get => processingMethod;
			set => processingMethod = value;
		}

		ISerializationDefinition IProcessor.Definition
		{
			get
			{
				switch (processingMethod)
				{
					case XmlPrimitiveProcessingMethod.ATTRIBUTES:
						return attributesProcessor.Definition;
					case XmlPrimitiveProcessingMethod.ELEMENTS:
						return elementsProcessor.Definition;
					default:
						throw new XmlException("Unsupported processing method ('{0}') to retrieve the serialization definition for.", processingMethod.ToString());
				}
			}
		}

		public UnityPrimitiveXmlSwitchProcessor(TAttributesProcessor attributesProcessor, TElementsProcessor elementsProcessor, XmlPrimitiveProcessingMethod preferredProcessingMethod)
		{
			attributesProcessor.ThrowIfNull(nameof(attributesProcessor));
			elementsProcessor.ThrowIfNull(nameof(elementsProcessor));

			this.attributesProcessor = attributesProcessor;
			this.elementsProcessor = elementsProcessor;
			this.processingMethod = preferredProcessingMethod;
		}

		public bool Serialize(object objectToSerialize, out object serializedResult)
		{
			switch (processingMethod)
			{
				case XmlPrimitiveProcessingMethod.ATTRIBUTES:
					return attributesProcessor.Serialize(objectToSerialize, out serializedResult);
				case XmlPrimitiveProcessingMethod.ELEMENTS:
					return elementsProcessor.Serialize(objectToSerialize, out serializedResult);
				default:
					throw new SerializationException("Unsupported processsing method ('{0}') to serialize the value.", processingMethod.ToString());
			}
		}

		public bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult)
		{
			targetType.ThrowIfNull(nameof(targetType));

			if (!(dataToDeserialize is XElement))
			{
				deserializedResult = null;
				return false;
			}

			// Depending whether the element has any children, decide how to process the data,
			// regardless of how the data prefers to be serialized.
			XElement element = dataToDeserialize as XElement;
			if (element.HasElements)
			{
				return elementsProcessor.Deserialize(targetType, dataToDeserialize, out deserializedResult);
			}
			else
			{
				return attributesProcessor.Deserialize(targetType, dataToDeserialize, out deserializedResult);
			}
		}
	}
}
