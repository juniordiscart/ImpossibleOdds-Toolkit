namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using System.Threading.Tasks;
	using ImpossibleOdds.Serialization.Caching;

	/// <summary>
	/// A (de)serialization processor for list-like data structures.
	/// </summary>
	public class SequenceProcessor : ISerializationProcessor, IDeserializationProcessor, IDeserializationToTargetProcessor
	{
		private readonly IIndexSerializationDefinition definition;
		private readonly IParallelProcessingSupport parallelProcessingSupport = null;

		/// <summary>
		/// Does the serialization definition have support for processing values in parallel?
		/// </summary>
		public bool SupportsParallelProcessing
		{
			get => parallelProcessingSupport != null;
		}

		/// <summary>
		/// The parallel processing serialization definition.
		/// </summary>
		public IParallelProcessingSupport ParallelProcessingDefinition
		{
			get => parallelProcessingSupport;
		}

		ISerializationDefinition IProcessor.Definition
		{
			get => definition;
		}

		IIndexSerializationDefinition Definition
		{
			get => definition;
		}

		public SequenceProcessor(IIndexSerializationDefinition definition)
		{
			this.definition = definition;
			this.parallelProcessingSupport = (definition is IParallelProcessingSupport parallelProcessingDefinition) ? parallelProcessingDefinition : null;
		}

		/// <summary>
		/// Attempts to serialize the object to another sequence in which all individual elements are processed by the serializer.
		/// </summary>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <param name="serializedResult">The serialized object.</param>
		/// <returns>True if the serialization is compatible and accepted, false otherwise.</returns>
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

			// Take the values from the original source values and process
			// them individually to data that is accepted by the serialization definition
			// and is accepted by the underlying type restrictions of the result collection.
			IList sourceValues = objectToSerialize as IList;
			IList processedValues = definition.CreateSequenceInstance(sourceValues.Count);
			SequenceCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(processedValues);
			object parallelLock = null;

			if (SupportsParallelProcessing && ParallelProcessingDefinition.Enabled && (sourceValues.Count > 1))
			{
				parallelLock = new object();
				Parallel.For(0, sourceValues.Count, SerializeMember);
			}
			else
			{
				for (int i = 0; i < sourceValues.Count; ++i)
				{
					SerializeMember(i);
				}
			}

			serializedResult = processedValues;
			return true;

			void SerializeMember(int index)
			{
				object processedValue = Serializer.Serialize(sourceValues[index], definition);
				if (parallelLock != null)
				{
					lock (parallelLock) SerializationUtilities.InsertInSequence(processedValues, collectionInfo, index, processedValue);
				}
				else
				{
					SerializationUtilities.InsertInSequence(processedValues, collectionInfo, index, processedValue);
				}
			}
		}

		/// <summary>
		/// Attempts to deserialize the data to a new instance of the target type. Each element of the data is deserialized individually by the deserialized.
		/// </summary>
		/// <param name="targetType">The target type to deserialize the given data.</param>
		/// <param name="dataToDeserialize">The data deserialize and apply to the result.</param>
		/// <param name="deserializedResult">The result unto which the data is applied.</param>
		/// <returns>True if deserialization is compatible and accepted, false otherwise.</returns>
		public bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult)
		{
			targetType.ThrowIfNull(nameof(targetType));

			// Check if the target implements the general IList interface, if not, we can just skip it altogether.
			if (!typeof(IList).IsAssignableFrom(targetType))
			{
				deserializedResult = null;
				return false;
			}

			// If the value is null, we can just assign it.
			if (dataToDeserialize == null)
			{
				deserializedResult = null;
				return true;
			}

			if (!(dataToDeserialize is IList))
			{
				throw new SerializationException("The source value is expected to implement the {0} interface to process to target type {1}.", typeof(IList).Name, targetType.Name);
			}

			// Arrays are treated differently.
			IList targetCollection =
				targetType.IsArray ?
				Array.CreateInstance(targetType.GetElementType(), (dataToDeserialize as IList).Count) :
				Activator.CreateInstance(targetType, true) as IList;

			if (!Deserialize(targetCollection, dataToDeserialize))
			{
				throw new SerializationException("Unexpected failure to process source value of type {0} to target collection of type {1}.", dataToDeserialize.GetType().Name, targetType.Name);
			}

			deserializedResult = targetCollection;
			return true;
		}

		/// <summary>
		/// Attempts to deserialize the data onto an existing instance. Each element of the data is deserialized individually by the deserialized.
		/// </summary>
		/// <param name="deserializationTarget">The object on which the data should be applied.</param>
		/// <param name="dataToDeserialize">The data to deserialize.</param>
		/// <returns>True if the deserialization is compatible and accepted, false otherwise.</returns>
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

			if (!(dataToDeserialize is IList))
			{
				throw new SerializationException("The source value is expected to implement the {0} interface to process to target instance of type {1}.", typeof(IList).Name, deserializationTarget.GetType().Name);
			}

			IList sourceValues = dataToDeserialize as IList;
			IList targetValues = deserializationTarget as IList;
			SequenceCollectionTypeInfo collectionInfo = SerializationUtilities.GetCollectionTypeInfo(targetValues);
			object parallelLock = null;

			if (SupportsParallelProcessing && ParallelProcessingDefinition.Enabled && (sourceValues.Count > 1))
			{
				parallelLock = new object();
				Parallel.For(0, sourceValues.Count, DeserializeMember);
			}
			else
			{
				for (int i = 0; i < sourceValues.Count; ++i)
				{
					DeserializeMember(i);
				}
			}

			return true;

			void DeserializeMember(int index)
			{
				object processedValue = Serializer.Deserialize(collectionInfo.elementType, sourceValues[index], definition);
				processedValue = collectionInfo.PostProcessValue(processedValue);
				if (parallelLock != null)
				{
					lock (parallelLock) SerializationUtilities.InsertInSequence(targetValues, collectionInfo, index, processedValue);
				}
				else
				{
					SerializationUtilities.InsertInSequence(targetValues, collectionInfo, index, processedValue);
				}
			}
		}
	}
}
