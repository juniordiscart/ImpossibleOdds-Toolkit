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

		public TAttributesProcessor AttributesProcessor
		{
			get => attributesProcessor;
		}

		public TElementsProcessor ElementsProcessor
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
					case XmlPrimitiveProcessingMethod.Attributes:
						return attributesProcessor.Definition;
					case XmlPrimitiveProcessingMethod.Elements:
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

		/// <inheritdoc />
		public virtual object Serialize(object objectToSerialize)
		{
			switch (processingMethod)
			{
				case XmlPrimitiveProcessingMethod.Attributes:
					return attributesProcessor.Serialize(objectToSerialize);
				case XmlPrimitiveProcessingMethod.Elements:
					return elementsProcessor.Serialize(objectToSerialize);
				default:
					throw new SerializationException("Unsupported processing method ('{0}') to serialize the value.", processingMethod.DisplayName());
			}
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			// Depending whether the element has any children, decide how to process the data,
			// regardless of how the data prefers to be serialized.
			XElement element = dataToDeserialize as XElement;
			if (element.HasElements)
			{
				return elementsProcessor.Deserialize(targetType, dataToDeserialize);
			}
			else if (element.HasAttributes)
			{
				return attributesProcessor.Deserialize(targetType, dataToDeserialize);
			}

			throw new SerializationException("Unsupported data of type {0} to deserialize into an instance of type {1}.", dataToDeserialize.GetType().Name, typeof(TPrimitive).Name);
		}

		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			if (!(objectToSerialize is TPrimitive))
			{
				return false;
			}

			switch(processingMethod)
			{
				case XmlPrimitiveProcessingMethod.Attributes:
					return AttributesProcessor.CanSerialize(objectToSerialize);
				case XmlPrimitiveProcessingMethod.Elements:
					return ElementsProcessor.CanSerialize(objectToSerialize);
				default:
					return false;
			}
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			if (!(dataToDeserialize is XElement))
			{
				return false;
			}

			switch(processingMethod)
			{
				case XmlPrimitiveProcessingMethod.Attributes:
					return AttributesProcessor.CanDeserialize(targetType, dataToDeserialize);
				case XmlPrimitiveProcessingMethod.Elements:
					return ElementsProcessor.CanDeserialize(targetType, dataToDeserialize);
				default:
					return false;
			}
		}
	}
}
