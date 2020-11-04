namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// A (de)serialization processor for dictionary-like data structures.
	/// </summary>
	public class LookupProcessor : AbstractCollectionProcessor, ISerializationProcessor, IDeserializationToTargetProcessor
	{
		private new ILookupSerializationDefinition definition;

		public LookupProcessor(ILookupSerializationDefinition definition, bool strictTypeChecking = DefaultOptions.StrictTypeChecking)
		: base(definition, strictTypeChecking)
		{
			this.definition = definition;
		}

		/// <summary>
		/// Attempts to serialize the given dictionary-like data structure to a data structure that is supported by the serialization definition. Each key-value pair is serialized individually as well.
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

			if (!typeof(IDictionary).IsAssignableFrom(objectToSerialize.GetType()))
			{
				serializedResult = null;
				return false;
			}

			IDictionary sourceValues = (IDictionary)objectToSerialize;
			Dictionary<object, object> processedValues = new Dictionary<object, object>(sourceValues.Count);

			// Process the source key and value pairs.
			ICollection keys = sourceValues.Keys;
			foreach (object key in keys)
			{
				object processedKey = Serializer.Serialize(key, definition);
				object processedValue = Serializer.Serialize(sourceValues[key], definition);
				processedValues.Add(processedKey, processedValue);
			}

			// Fill up the result with the processed keys and values.
			IDictionary resultCollection = definition.CreateLookupInstance(sourceValues.Count);
			SerializationUtilities.FillLookup(processedValues, resultCollection);
			serializedResult = resultCollection;
			return true;
		}

		/// <summary>
		/// Attempts to deserialize the given dictionary-like data structure and apply the data onto a new instance of the target type. Each key-valye pair is deserialized individually as well.
		/// </summary>
		/// <param name="targetType">The target type to deserialize the given data.</param>
		/// <param name="dataToDeserialize">The data deserialize and apply to the result.</param>
		/// <param name="deserializedResult">The result unto which the data is applied.</param>
		/// <returns>True if deserialization is compatible and accepted, false otherwise.</returns>
		public bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult)
		{
			// Check if the target implements the general IDictionary interface, if not, we can just skip altogether.
			if ((targetType == null) || !typeof(IDictionary).IsAssignableFrom(targetType))
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

			if (targetType.IsAbstract || targetType.IsInterface)
			{
				throw new SerializationException(string.Format("Target type {0} is abstract or an interface. Cannot create an instance to process the source values.", targetType.Name));
			}
			else if (!typeof(IDictionary).IsAssignableFrom(dataToDeserialize.GetType()))
			{
				throw new SerializationException(string.Format("The source value is expected to implement the {0} interface to process to target type {1}.", typeof(IDictionary), targetType.Name));
			}

			IDictionary targetCollection = Activator.CreateInstance(targetType, true) as IDictionary;
			if (!Deserialize(targetCollection, dataToDeserialize))
			{
				throw new SerializationException(string.Format("Unexpected failure to process source value of type {0} to target collection of type {1}.", dataToDeserialize.GetType().Name, targetType.Name));
			}

			deserializedResult = targetCollection;
			return true;
		}

		/// <summary>
		/// Attempts to deserialize the given dictionary-like data structure and apply the data onto the given target. Each key-valye pair is deserialized individually as well.
		/// </summary>
		/// <param name="deserializationTarget">The object on which the data should be applied.</param>
		/// <param name="dataToDeserialize">The data to deserialize.</param>
		/// <returns>True if the deserialization is compatible and accepted, false otherwise.</returns>
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

			if (!typeof(IDictionary).IsAssignableFrom(dataToDeserialize.GetType()))
			{
				throw new SerializationException(string.Format("The source value is expected to implement the {0} interface to process to target instance of type {1}.", typeof(IDictionary), deserializationTarget.GetType().Name));
			}

			Type targetType = deserializationTarget.GetType();
			IDictionary targetCollection = deserializationTarget as IDictionary;

			// Check whether the target implements a generic variant and if the arguments apply additional type restrictions.
			Type genericType = SerializationUtilities.GetGenericType(targetType, typeof(IDictionary<,>));
			Type[] genericParams = (genericType != null) ? genericType.GetGenericArguments() : null;
			bool isKeyTypeConstrained = (genericParams != null) && (genericParams[0] != typeof(object));
			bool isValueTypeConstrained = (genericParams != null) && (genericParams[1] != typeof(object));

			Type keyType = isKeyTypeConstrained ? genericParams[0] : typeof(object);
			Type valueType = isValueTypeConstrained ? genericParams[1] : typeof(object);

			// Process each key-value pair of the source values individually.
			IDictionary sourceValues = dataToDeserialize as IDictionary;
			ICollection keys = sourceValues.Keys;
			foreach (object key in keys)
			{
				object processedKey = Serializer.Deserialize(keyType, key, definition);
				processedKey = SerializationUtilities.PostProcessValue(processedKey, keyType);
				if ((processedKey == null) || (isKeyTypeConstrained && !PassesTypeRestriction(processedKey, keyType)))
				{
					Log.Warning("A key of type {0} could not be processed to a valid key for target of type {1}. Skipping key and value.", key.GetType(), targetType.Name);
					continue;
				}

				object processedValue = Serializer.Deserialize(valueType, sourceValues[key], definition);
				processedValue = SerializationUtilities.PostProcessValue(processedValue, valueType);
				if (isValueTypeConstrained && !PassesTypeRestriction(processedValue, valueType))
				{
					if (sourceValues[key] == null)
					{
						Log.Warning("A null value could not be processed to a valid value for target of type {0}. Skipping key and value.", targetType.Name);
					}
					else
					{
						Log.Warning("A value of type {0} could not be processed to a valid value for target of type {1}. Skipping key and value.", sourceValues[key].GetType().Name, targetType.Name);
					}
					continue;
				}

				targetCollection.Add(processedKey, processedValue);
			}

			return true;
		}
	}
}
