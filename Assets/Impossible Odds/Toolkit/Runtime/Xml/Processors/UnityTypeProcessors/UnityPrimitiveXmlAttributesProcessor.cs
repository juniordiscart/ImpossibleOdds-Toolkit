namespace ImpossibleOdds.Xml.Processors
{
	using System;
	using System.Xml.Linq;
	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Serialization.Processors;

	public abstract class UnityPrimitiveXmlAttributesProcessor<TPrimitive> : ISerializationProcessor, IDeserializationProcessor
	{
		private readonly XmlSerializationDefinition definition = null;

		public XmlSerializationDefinition Definition
		{
			get => definition;
		}

		ISerializationDefinition IProcessor.Definition
		{
			get => definition;
		}

		public UnityPrimitiveXmlAttributesProcessor(XmlSerializationDefinition definition)
		{
			this.definition = definition;
		}

		/// <inheritdoc />
		public virtual object Serialize(object objectToSerialize)
		{
			return Serialize((TPrimitive)objectToSerialize);
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			return Deserialize(dataToDeserialize as XElement);
		}

		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			return
				(objectToSerialize != null) &&
				(objectToSerialize is TPrimitive primitive) &&
				CanSerialize(primitive);
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));
			
			return
				(dataToDeserialize != null) &&
				(dataToDeserialize is XElement element) &&
				typeof(TPrimitive).IsAssignableFrom(targetType) &&
				CanDeserialize(element);
		}

		protected abstract bool CanSerialize(TPrimitive primitive);
		protected abstract bool CanDeserialize(XElement element);
		protected abstract XElement Serialize(TPrimitive value);
		protected abstract TPrimitive Deserialize(XElement xmlData);
	}
}
