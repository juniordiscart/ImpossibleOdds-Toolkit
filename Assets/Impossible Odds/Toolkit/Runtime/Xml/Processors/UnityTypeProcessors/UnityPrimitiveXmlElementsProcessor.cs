using System;
using System.Xml.Linq;
using ImpossibleOdds.Serialization;
using ImpossibleOdds.Serialization.Processors;

namespace ImpossibleOdds.Xml.Processors
{
	public abstract class UnityPrimitiveXmlElementsProcessor<TPrimitive> : ISerializationProcessor, IDeserializationProcessor
	{
		public ISerializationDefinition Definition { get; }
		
		public abstract string[] Keys { get; }

		public UnityPrimitiveXmlElementsProcessor(XmlSerializationDefinition definition)
		{
			definition.ThrowIfNull(nameof(definition));
			Definition = definition;
		}

		/// <inheritdoc />
		public virtual object Serialize(object objectToSerialize)
		{
			this.ThrowIfCantSerialize(objectToSerialize);
			return Serialize((TPrimitive)objectToSerialize);
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			this.ThrowIfCantDeserialize(targetType, dataToDeserialize);
			return Deserialize((XElement)dataToDeserialize);
		}

		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			return objectToSerialize is TPrimitive;
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			return
				(dataToDeserialize is XElement element) &&
				typeof(TPrimitive).IsAssignableFrom(targetType) &&
				Array.TrueForAll(Keys, key => element.Element(key) != null);
		}
		
		protected abstract XElement Serialize(TPrimitive value);
		protected abstract TPrimitive Deserialize(XElement xmlData);
	}
}