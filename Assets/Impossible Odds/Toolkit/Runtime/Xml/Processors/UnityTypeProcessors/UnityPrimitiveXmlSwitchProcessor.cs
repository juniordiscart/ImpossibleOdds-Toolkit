using System;
using System.Xml.Linq;
using ImpossibleOdds.Serialization;
using ImpossibleOdds.Serialization.Processors;

namespace ImpossibleOdds.Xml.Processors
{
	public abstract class UnityPrimitiveXmlSwitchProcessor<TAttributesProcessor, TElementsProcessor, TPrimitive> : IUnityPrimitiveXmlSwitchProcessor, ISerializationProcessor, IDeserializationProcessor
	where TAttributesProcessor : UnityPrimitiveXmlAttributesProcessor<TPrimitive>
	where TElementsProcessor : UnityPrimitiveXmlElementsProcessor<TPrimitive>
	{
		public TAttributesProcessor AttributesProcessor { get; }

		public TElementsProcessor ElementsProcessor { get; }

		public XmlPrimitiveProcessingMethod ProcessingMethod { get; set; }

		ISerializationDefinition IProcessor.Definition
		{
			get
			{
				return ProcessingMethod switch
				{
					XmlPrimitiveProcessingMethod.Attributes => AttributesProcessor.Definition,
					XmlPrimitiveProcessingMethod.Elements => ElementsProcessor.Definition,
					_ => throw new XmlException("Unsupported processing method ('{0}') to retrieve the serialization definition for.", ProcessingMethod.ToString())
				};
			}
		}

		public UnityPrimitiveXmlSwitchProcessor(TAttributesProcessor attributesProcessor, TElementsProcessor elementsProcessor, XmlPrimitiveProcessingMethod preferredProcessingMethod)
		{
			attributesProcessor.ThrowIfNull(nameof(attributesProcessor));
			elementsProcessor.ThrowIfNull(nameof(elementsProcessor));

			AttributesProcessor = attributesProcessor;
			ElementsProcessor = elementsProcessor;
			ProcessingMethod = preferredProcessingMethod;
		}

		/// <inheritdoc />
		public virtual object Serialize(object objectToSerialize)
		{
			return ProcessingMethod switch
			{
				XmlPrimitiveProcessingMethod.Attributes => AttributesProcessor.Serialize(objectToSerialize),
				XmlPrimitiveProcessingMethod.Elements => ElementsProcessor.Serialize(objectToSerialize),
				_ => throw new SerializationException($"Unsupported processing method ('{ProcessingMethod.DisplayName()}') to serialize the value.")
			};
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			// Depending whether the element has any children, decide how to process the data,
			// regardless of how the data prefers to be serialized.
			XElement element = (XElement)dataToDeserialize;
			if (element.HasElements)
			{
				return ElementsProcessor.Deserialize(targetType, dataToDeserialize);
			}

			if (element.HasAttributes)
			{
				return AttributesProcessor.Deserialize(targetType, dataToDeserialize);
			}

			throw new SerializationException($"Unsupported data of type {dataToDeserialize.GetType().Name} to deserialize into an instance of type {typeof(TPrimitive).Name}.");
		}

		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			if (!(objectToSerialize is TPrimitive))
			{
				return false;
			}

			return ProcessingMethod switch
			{
				XmlPrimitiveProcessingMethod.Attributes => AttributesProcessor.CanSerialize(objectToSerialize),
				XmlPrimitiveProcessingMethod.Elements => ElementsProcessor.CanSerialize(objectToSerialize),
				_ => false
			};
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			if (!(dataToDeserialize is XElement))
			{
				return false;
			}

			return ProcessingMethod switch
			{
				XmlPrimitiveProcessingMethod.Attributes => AttributesProcessor.CanDeserialize(targetType, dataToDeserialize),
				XmlPrimitiveProcessingMethod.Elements => ElementsProcessor.CanDeserialize(targetType, dataToDeserialize),
				_ => false
			};
		}
	}
}