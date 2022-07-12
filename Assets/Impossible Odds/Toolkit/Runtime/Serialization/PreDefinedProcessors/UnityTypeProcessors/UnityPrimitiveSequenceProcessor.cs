﻿namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;

	public abstract class UnityPrimitiveSequenceProcessor<T> : ISerializationProcessor, IDeserializationProcessor
	{
		private readonly IIndexSerializationDefinition definition = null;

		public IIndexSerializationDefinition Definition
		{
			get => definition;
		}

		ISerializationDefinition IProcessor.Definition
		{
			get => Definition;
		}

		public UnityPrimitiveSequenceProcessor(IIndexSerializationDefinition definition)
		{
			this.definition = definition;
		}

		/// <summary>
		/// Serializes a Unity primitive of type T to a sequence data structure.
		/// </summary>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <param name="serializedResult">The serialized result.</param>
		/// <returns>True if the serialization is compatible and accepted, false otherwise.</returns>
		public bool Serialize(object objectToSerialize, out object serializedResult)
		{
			if ((objectToSerialize == null) || !(objectToSerialize is T dataToSerialize))
			{
				serializedResult = null;
				return false;
			}

			serializedResult = Serialize(dataToSerialize);
			return true;
		}

		/// <summary>
		/// Attempts to deserialize the object as a Unity primitive object of type T.
		/// </summary>
		/// <param name="targetType">The target type to deserialize the given data to.</param>
		/// <param name="dataToDeserialize">The data to deserialize.</param>
		/// <param name="deserializedResult">The result unto which the data is applied.</param>
		/// <returns>True if deserialization is compatible and accepted, false otherwise.</returns>
		public bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult)
		{
			targetType.ThrowIfNull(nameof(targetType));

			if ((typeof(T) != targetType) || !(dataToDeserialize is IList sequenceData))
			{
				deserializedResult = null;
				return false;
			}

			deserializedResult = Deserialize(sequenceData);
			return true;
		}

		protected abstract IList Serialize(T value);
		protected abstract T Deserialize(IList sequenceData);
	}
}
