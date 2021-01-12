namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;

	/// <summary>
	/// Abstract processor for common Unity types.
	/// </summary>
	/// <typeparam name="T">A Unity pimitive type.</typeparam>
	public abstract class AbstractUnityPrimitiveProcessor<T> : ISerializationProcessor, IDeserializationProcessor
	{
		private bool preferSequenceOverLookup = false;
		private ISerializationDefinition definition = null;

		public ISerializationDefinition Definition
		{
			get { return definition; }
		}

		private IIndexSerializationDefinition IndexBasedDefinition
		{
			get { return (IIndexSerializationDefinition)definition; }
		}

		private ILookupSerializationDefinition LookupBasedDefinition
		{
			get { return (ILookupSerializationDefinition)definition; }
		}

		/// <summary>
		/// Initialize this processor with an index-based serialization definition. All instances provided
		/// to be serialized will be transformed to the requested sequence-like data structure.
		/// </summary>
		/// <param name="definition"></param>
		protected AbstractUnityPrimitiveProcessor(IIndexSerializationDefinition definition)
		{
			this.preferSequenceOverLookup = true;
			this.definition = definition;
		}

		/// <summary>
		/// Initialize this process with a lookup-based serialization definition. All instances provided
		/// to be serialized will be transformed to the requested lookup-like data structure.
		/// </summary>
		/// <param name="definition"></param>
		protected AbstractUnityPrimitiveProcessor(ILookupSerializationDefinition definition)
		{
			this.preferSequenceOverLookup = false;
			this.definition = definition;
		}

		/// <summary>
		/// Attempt to serialize the object to a single instance of the Unity-primitive type.
		/// </summary>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <param name="serializedResult">The serialized result.</param>
		/// <returns>True if the serialization is compatible and accepted, false otherwise.</returns>
		public virtual bool Serialize(object objectToSerialize, out object serializedResult)
		{
			// Perform an exact type-check, because Unity has several implicit operators defined for its internal types.
			if ((objectToSerialize == null) || (typeof(T) != objectToSerialize.GetType()))
			{
				serializedResult = null;
				return false;
			}

			if (preferSequenceOverLookup)
			{
				IList values = ValueToSequence((T)objectToSerialize);
				IList resultCollection = IndexBasedDefinition.CreateSequenceInstance(values.Count);
				SerializationUtilities.FillSequence(values, resultCollection);
				serializedResult = resultCollection;
			}
			else
			{
				IDictionary values = ValueToLookup((T)objectToSerialize);
				IDictionary resultCollection = LookupBasedDefinition.CreateLookupInstance(values.Count);
				SerializationUtilities.FillLookup(values, resultCollection);
				serializedResult = resultCollection;
			}

			return true;
		}

		/// <summary>
		/// Attempts to deserialize the object to an instance of the Unity-primitive type.
		/// </summary>
		/// <param name="targetType">The target type to deserialize the given data to.</param>
		/// <param name="dataToDeserialize">The data to deserialize.</param>
		/// <param name="deserializedResult">The result unto which the data is applied.</param>
		/// <returns>True if deserialization is compatible and accepted, false otherwise.</returns>
		public virtual bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult)
		{
			if ((targetType == null) || (typeof(T) != targetType))
			{
				deserializedResult = null;
				return false;
			}

			if (dataToDeserialize is IDictionary lookupCollection)
			{
				deserializedResult = LookupToValue(lookupCollection);
			}
			else if (dataToDeserialize is IList sequenceCollection)
			{
				deserializedResult = SequenceToValue(sequenceCollection);
			}
			else
			{
				throw new SerializationException("The data to deserialize is neither an instance of '{0}' or '{1}'.", typeof(IList).Name, typeof(IDictionary).Name);
			}

			return true;
		}

		protected abstract IList ValueToSequence(T value);
		protected abstract IDictionary ValueToLookup(T value);
		protected abstract T SequenceToValue(IList sequence);
		protected abstract T LookupToValue(IDictionary lookup);
	}
}
