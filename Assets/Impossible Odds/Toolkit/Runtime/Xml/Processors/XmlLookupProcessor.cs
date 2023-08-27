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
		public XmlSerializationDefinition Definition { get; } = null;

		public IParallelProcessingFeature ParallelProcessingFeature { get; set; }
		public bool SupportsParallelProcessing => ParallelProcessingFeature != null;

		ISerializationDefinition IProcessor.Definition => Definition;

		public XmlLookupProcessor(XmlSerializationDefinition definition)
		{
			definition.ThrowIfNull(nameof(definition));
			Definition = definition;
		}

		/// <inheritdoc />
		public object Serialize(object objectToSerialize)
		{
			// Accept null values.
			if ((objectToSerialize == null))
			{
				return null;
			}

			// Process each individual lookup entry.
			XElement lookupRoot = new XElement("LookupElement");   // Create default-named root.
			IDictionary sourceValues = (IDictionary)objectToSerialize;
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

			return lookupRoot;

			void SerializeMember(DictionaryEntry sourceValue)
			{
				object processedKey = Serializer.Serialize(sourceValue.Key, Definition);
				object processedValue = Serializer.Serialize(sourceValue.Value, Definition);

				if (!(processedKey is string))
				{
					processedKey = Convert.ToString(processedKey);
				}

				// If the value is already an XElement, then just change
				// the name of the element to that of the key.
				XElement xmlEntry;
				if (processedValue is XElement xElement)
				{
					xmlEntry = xElement;
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

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			// If the value is null, it can just return already.
			if (dataToDeserialize == null)
			{
				return null;
			}

			IDictionary targetCollection = Activator.CreateInstance(targetType, true) as IDictionary;
			Deserialize(targetCollection, dataToDeserialize);
			return dataToDeserialize;
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
			IDictionary targetValues = (IDictionary)deserializationTarget;
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

			void DeserializeMember(XElement xmlEntry)
			{
				object processedKey = Serializer.Deserialize(collectionInfo.keyType, xmlEntry.Name.LocalName, Definition);

				// If the value has any child elements or attributes, then the entry itself is deserialized, otherwise just it value is chosen.
				object processedValue = (xmlEntry.HasElements || xmlEntry.HasAttributes) ? xmlEntry : (object)xmlEntry.Value;
				processedValue = Serializer.Deserialize(collectionInfo.valueType, processedValue, Definition);

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
	
		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			return
				(objectToSerialize == null) ||
				(objectToSerialize is IDictionary);
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			// Check if the target implements the general IDictionary interface, if not, we can just skip altogether.
			return
				typeof(IDictionary).IsAssignableFrom(targetType) &&
				((dataToDeserialize == null) || (dataToDeserialize is IDictionary));
		}
	}
}