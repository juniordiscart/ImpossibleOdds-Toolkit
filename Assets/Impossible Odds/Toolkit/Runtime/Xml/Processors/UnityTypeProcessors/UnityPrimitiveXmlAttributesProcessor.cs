using System;
using System.Xml.Linq;
using ImpossibleOdds.Serialization;
using ImpossibleOdds.Serialization.Processors;

namespace ImpossibleOdds.Xml.Processors
{
	public abstract class UnityPrimitiveXmlAttributesProcessor<TPrimitive> : ISerializationProcessor, IDeserializationProcessor
	{
		public ISerializationDefinition Definition { get; }

		protected UnityPrimitiveXmlAttributesProcessor(XmlSerializationDefinition definition)
		{
			definition.ThrowIfNull(nameof(definition));
			Definition = definition;
		}

		/// <inheritdoc />
		public virtual object Serialize(object objectToSerialize)
		{
			return Serialize((TPrimitive)objectToSerialize);
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			return Deserialize((XElement)dataToDeserialize);
		}

		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			return (objectToSerialize is TPrimitive primitive) && CanSerialize(primitive);
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));
			
			return
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