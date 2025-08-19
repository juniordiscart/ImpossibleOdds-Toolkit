using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using ImpossibleOdds.Serialization;
using ImpossibleOdds.Serialization.Caching;
using ImpossibleOdds.Serialization.Processors;

namespace ImpossibleOdds.Xml.Processors
{
	public class XmlSequenceProcessor : ISerializationProcessor, IDeserializationToTargetProcessor
	{
		public ISerializationDefinition Definition { get; }
		public IParallelProcessingFeature ParallelProcessingFeature { get; set; }
		
		private bool ParallelProcessingEnabled => ParallelProcessingFeature is { Enabled: true };

		public XmlSequenceProcessor(XmlSerializationDefinition definition)
		{
			definition.ThrowIfNull(nameof(definition));
			Definition = definition;
		}

		/// <inheritdoc />
		public virtual object Serialize(object objectToSerialize)
		{
			this.ThrowIfCantSerialize(objectToSerialize);
			
			// Accept null values.
			if ((objectToSerialize == null))
			{
				return null;
			}

			// Process each entry in the list to a xml element.
			XElement listRoot = new XElement("ListElement"); // Create a default-named list-root element.
			IList sourceValues = (IList)objectToSerialize;

			if (ParallelProcessingEnabled && (sourceValues.Count > 1))
			{
				Parallel.For(0, sourceValues.Count, index =>
				{
					object processedValue = Serializer.Serialize(sourceValues[index], Definition);

					// If the processed value is not yet a xml element already, then create one.
					XElement xmlEntry = processedValue as XElement ?? new XElement(XmlListElementAttribute.DefaultListEntryName, processedValue);
					lock(listRoot) listRoot.Add(xmlEntry);
				});
			}
			else
			{
				foreach (object sourceValue in sourceValues)
				{
					object processedValue = Serializer.Serialize(sourceValue, Definition);

					// If the processed value is not yet a xml element already, then create one.
					XElement xmlEntry = processedValue as XElement ?? new XElement(XmlListElementAttribute.DefaultListEntryName, processedValue);
					listRoot.Add(xmlEntry);
				}
			}
			

			return listRoot;
		}

		/// <inheritdoc />
		public virtual object Deserialize(Type targetType, object dataToDeserialize)
		{
			this.ThrowIfCantDeserialize(targetType, dataToDeserialize);
			
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
			this.ThrowIfCantDeserializeToTarget(deserializationTarget, dataToDeserialize);
			
			// If there is nothing to do...
			if (dataToDeserialize == null)
			{
				return;
			}

			XElement sourceXml = (XElement)dataToDeserialize;
			IList targetValues = (IList)deserializationTarget;
			ListCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(targetValues);
			XElement[] sourceElements = sourceXml.Elements().ToArray();

			if (ParallelProcessingEnabled && (sourceElements.Length > 1))
			{
				Parallel.For(0, sourceElements.Length, index =>
				{
					// If the value has any child elements or attributes, then the entry itself is deserialized, otherwise just its value is chosen.
					XElement xmlEntry = sourceElements[index];
					object processedValue = (xmlEntry.HasElements || xmlEntry.HasAttributes) ? xmlEntry : xmlEntry.Value;
					processedValue = Serializer.Deserialize(collectionInfo.elementType, processedValue, Definition);

					lock (targetValues) SerializationUtilities.InsertInList(targetValues, collectionInfo, index, processedValue);
				});
			}
			else
			{
				for (int index = 0; index < sourceElements.Length; index++)
				{
					// If the value has any child elements or attributes, then the entry itself is deserialized, otherwise just its value is chosen.
					XElement xmlEntry = sourceElements[index];
					object processedValue = (xmlEntry.HasElements || xmlEntry.HasAttributes) ? xmlEntry : xmlEntry.Value;
					processedValue = Serializer.Deserialize(collectionInfo.elementType, processedValue, Definition);

					SerializationUtilities.InsertInList(targetValues, collectionInfo, index, processedValue);
				}
			}
		}
	
		/// <inheritdoc />
		public virtual bool CanSerialize(object objectToSerialize)
		{
			return objectToSerialize is null or IList;
		}

		/// <inheritdoc />
		public virtual bool CanDeserialize(Type targetType, object dataToDeserialize)
		{
			targetType.ThrowIfNull(nameof(targetType));

			return typeof(IList).IsAssignableFrom(targetType) && dataToDeserialize is null or XElement;
		}
		
		/// <inheritdoc />
		public virtual bool CanDeserialize(object deserializationTarget, object dataToDeserialize)
		{
			return deserializationTarget != null && CanDeserialize(deserializationTarget.GetType(), dataToDeserialize);
		}
	}
}