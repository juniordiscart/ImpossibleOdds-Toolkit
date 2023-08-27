namespace ImpossibleOdds.Xml.Processors
{
	using System;
	using System.Collections;
	using System.Linq;
	using System.Xml.Linq;
	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Serialization.Caching;
	using ImpossibleOdds.Serialization.Processors;

	public class XmlSequenceProcessor : ISerializationProcessor, IDeserializationProcessor, IDeserializationToTargetProcessor
	{
		public XmlSerializationDefinition Definition { get; } = null;
		public IParallelProcessingFeature ParallelProcessingFeature { get; set; }
		public bool SupportsParallelProcessing => ParallelProcessingFeature != null;

		ISerializationDefinition IProcessor.Definition => Definition;

		public XmlSequenceProcessor(XmlSerializationDefinition definition)
		{
			definition.ThrowIfNull(nameof(definition));
			Definition = definition;
		}

		/// <inheritdoc />
		public virtual object Serialize(object objectToSerialize)
		{
			// Accept null values.
			if ((objectToSerialize == null))
			{
				return null;
			}

			// Process each entry in the list to an xml element.
			XElement listRoot = new XElement("List"); // Create a default-named list-root element.
			IList sourceValues = (IList)objectToSerialize;
			foreach (object sourceValue in sourceValues)
			{
				object processedValue = Serializer.Serialize(sourceValue, Definition);

				// If the processed value is not yet an xml element already, then create one.
				XElement xmlEntry = (processedValue is XElement) ? (processedValue as XElement) : new XElement(XmlListElementAttribute.DefaultListEntryName, processedValue);
				listRoot.Add(xmlEntry);
			}

			return listRoot;
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			// If the value is null, it can just return here already.
			if (dataToDeserialize == null)
			{
				return null;
			}

			// Arrays are treated differently.
			IList targetCollection =
				targetType.IsArray ?
				Array.CreateInstance(targetType.GetElementType(), ((XElement)dataToDeserialize).Elements().Count()) :
				Activator.CreateInstance(targetType, true) as IList;

			Deserialize(targetCollection, dataToDeserialize);
			return targetCollection;
		}

		/// <inheritdoc />
		public virtual void Deserialize(object deserializationTarget, object dataToDeserialize)
		{
			// If there is nothing to do...
			if (dataToDeserialize == null)
			{
				return;
			}

			XElement sourceXml = (XElement)dataToDeserialize;
			IList targetValues = (IList)deserializationTarget;
			SequenceCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(targetValues);

			int i = 0;
			foreach (XElement xmlEntry in sourceXml.Elements())
			{
				// If the value has any child elements or attributes, then the entry itself is deserialized, otherwise just its value is chosen.
				object processedValue = (xmlEntry.HasElements || xmlEntry.HasAttributes) ? (object)xmlEntry : (object)xmlEntry.Value;
				processedValue = Serializer.Deserialize(collectionInfo.elementType, processedValue, Definition);

				SerializationUtilities.InsertInSequence(targetValues, collectionInfo, i, processedValue);
				++i;
			}
		}
	
		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			return
				(objectToSerialize == null) ||
				(objectToSerialize is IList);
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			return
				typeof(IList).IsAssignableFrom(targetType) &&
				((dataToDeserialize == null) || (dataToDeserialize is XElement));
		}
	}
}