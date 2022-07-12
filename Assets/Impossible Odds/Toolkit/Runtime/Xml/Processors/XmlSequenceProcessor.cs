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
		private readonly XmlSerializationDefinition definition = null;

		public XmlSerializationDefinition Definition
		{
			get => definition;
		}

		ISerializationDefinition IProcessor.Definition
		{
			get => Definition;
		}

		public XmlSequenceProcessor(XmlSerializationDefinition definition)
		{
			definition.ThrowIfNull(nameof(definition));
			this.definition = definition;
		}

		public bool Serialize(object objectToSerialize, out object serializedResult)
		{
			// Accept null values.
			if ((objectToSerialize == null))
			{
				serializedResult = null;
				return true;
			}

			if (!(objectToSerialize is IList))
			{
				serializedResult = null;
				return false;
			}

			// Process each entry in the list to an xml element.
			XElement listRoot = new XElement("List"); // Create a default-named list-root element.
			IList sourceValues = objectToSerialize as IList;
			foreach (object sourceValue in sourceValues)
			{
				object processedValue = Serializer.Serialize(sourceValue, definition);

				// If the processed value is not yet an xml element already, then create one.
				XElement xmlEntry = (processedValue is XElement) ? (processedValue as XElement) : new XElement(XmlListElementAttribute.DefaultListEntryName, processedValue);
				listRoot.Add(xmlEntry);
			}

			serializedResult = listRoot;
			return true;
		}

		public bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult)
		{
			targetType.ThrowIfNull(nameof(targetType));

			// Check if the target implements the general IList interface, if not, we can just skip altogether.
			if (!typeof(IList).IsAssignableFrom(targetType))
			{
				deserializedResult = null;
				return false;
			}

			// If the value is null, it can just be assigned.
			if (dataToDeserialize == null)
			{
				deserializedResult = null;
				return true;
			}

			if (!(dataToDeserialize is XElement))
			{
				throw new XmlException("The value to deserialize is not of type {0}.", typeof(XElement).Name);
			}

			// Arrays are treated differently.
			IList targetCollection =
				targetType.IsArray ?
				Array.CreateInstance(targetType.GetElementType(), (dataToDeserialize as XElement).Elements().Count()) :
				Activator.CreateInstance(targetType, true) as IList;

			if (!Deserialize(targetCollection, dataToDeserialize))
			{
				throw new XmlException("Unexpected failure to process source value of type {0} to target collection of type {1}.", dataToDeserialize.GetType().Name, targetType.Name);
			}

			deserializedResult = targetCollection;
			return true;
		}

		public bool Deserialize(object deserializationTarget, object dataToDeserialize)
		{
			if ((deserializationTarget == null) || !(deserializationTarget is IList))
			{
				return false;
			}

			// If there is nothing to do...
			if (dataToDeserialize == null)
			{
				return true;
			}

			if (!(dataToDeserialize is XElement))
			{
				throw new XmlException("The source value is expected to be of type {0} in order to process to a target instance of type {1}.", typeof(XElement).Name, deserializationTarget.GetType().Name);
			}

			XElement sourceXml = dataToDeserialize as XElement;
			IList targetValues = deserializationTarget as IList;
			SequenceCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(targetValues);

			int i = 0;
			foreach (XElement xmlEntry in sourceXml.Elements())
			{
				// If the value has any child elements or attributes, then the entry itself is deserialized, otherwise just its value is chosen.
				object processedValue = (xmlEntry.HasElements || xmlEntry.HasAttributes) ? (object)xmlEntry : (object)xmlEntry.Value;
				processedValue = Serializer.Deserialize(collectionInfo.elementType, processedValue, definition);

				SerializationUtilities.InsertInSequence(targetValues, collectionInfo, i, processedValue);
				++i;
			}

			return true;
		}
	}
}
