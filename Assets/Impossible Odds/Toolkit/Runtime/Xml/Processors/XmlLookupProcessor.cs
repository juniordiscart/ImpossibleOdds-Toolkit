namespace ImpossibleOdds.Xml.Processors
{
	using System;
	using System.Collections;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Xml.Linq;
	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Serialization.Caching;
	using ImpossibleOdds.Serialization.Processors;
	using ImpossibleOdds.Xml;

	public class XmlLookupProcessor : ISerializationProcessor, IDeserializationProcessor, IDeserializationToTargetProcessor
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

		public XmlLookupProcessor(XmlSerializationDefinition definition)
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

			if (!(objectToSerialize is IDictionary))
			{
				serializedResult = null;
				return false;
			}

			// Process each individual lookup entry.
			XElement lookupRoot = new XElement("LookupElement");   // Create default-named root.
			IDictionary sourceValues = objectToSerialize as IDictionary;
			object parallelLock = null;

			if (Definition.ParallelProcessingEnabled && (sourceValues.Count > 1))
			{
				parallelLock = new object();
				Parallel.ForEach(sourceValues.Cast<DictionaryEntry>(), SerializeMember);
			}
			else
			{
				foreach (DictionaryEntry sourceValue in sourceValues)
				{
					SerializeMember(sourceValue);
				}
			}

			serializedResult = lookupRoot;
			return true;

			void SerializeMember(DictionaryEntry sourceValue)
			{
				object processedKey = Serializer.Serialize(sourceValue.Key, definition);
				object processedValue = Serializer.Serialize(sourceValue.Value, definition);

				if (!(processedKey is string))
				{
					processedKey = Convert.ToString(processedKey);
				}

				// If the value is already an XElement, then just change
				// the name of the element to that of the key.
				XElement xmlEntry = null;
				if (processedValue is XElement)
				{
					xmlEntry = processedValue as XElement;
					xmlEntry.Name = processedKey as string;
				}
				else
				{
					xmlEntry = new XElement(processedKey as string, processedValue);
				}

				if (parallelLock != null)
				{
					lock (parallelLock) lookupRoot.Add(xmlEntry);
				}
				else
				{
					lookupRoot.Add(xmlEntry);
				}
			}
		}

		public bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult)
		{
			targetType.ThrowIfNull(nameof(targetType));

			// Check if the target implements the general IDictionary interface, if not, we can just skip altogether.
			if (!typeof(IDictionary).IsAssignableFrom(targetType))
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

			IDictionary targetCollection = Activator.CreateInstance(targetType, true) as IDictionary;
			if (!Deserialize(targetCollection, dataToDeserialize))
			{
				throw new XmlException("Unexpected failure to process source value of type {0} to target collection of type {1}.", dataToDeserialize.GetType().Name, targetType.Name);
			}

			deserializedResult = targetCollection;
			return true;
		}

		public bool Deserialize(object deserializationTarget, object dataToDeserialize)
		{
			if ((deserializationTarget == null) || !(deserializationTarget is IDictionary))
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
			IDictionary targetValues = deserializationTarget as IDictionary;
			LookupCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(targetValues);
			object parallelLock = null;

			if (Definition.ParallelProcessingEnabled && (targetValues.Count > 1))
			{
				parallelLock = new object();
				Parallel.ForEach(sourceXml.Elements(), DeserializeMember);
			}
			else
			{
				foreach (XElement xmlEntry in sourceXml.Elements())
				{
					DeserializeMember(xmlEntry);
				}
			}

			return true;

			void DeserializeMember(XElement xmlEntry)
			{
				object processedKey = Serializer.Deserialize(collectionInfo.keyType, xmlEntry.Name.LocalName, definition);

				// If the value has any child elements or attributes, then the entry itself is deserialized, otherwise just it value is chosen.
				object processedValue = (xmlEntry.HasElements || xmlEntry.HasAttributes) ? (object)xmlEntry : (object)xmlEntry.Value;
				processedValue = Serializer.Deserialize(collectionInfo.valueType, processedValue, definition);

				if (parallelLock != null)
				{
					lock (parallelLock) SerializationUtilities.InsertInLookup(targetValues, collectionInfo, processedKey, processedValue);
				}
				else
				{
					SerializationUtilities.InsertInLookup(targetValues, collectionInfo, processedKey, processedValue);
				}
			}
		}
	}
}
