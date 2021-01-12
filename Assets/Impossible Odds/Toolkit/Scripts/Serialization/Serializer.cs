namespace ImpossibleOdds.Serialization
{
	using System;
	using ImpossibleOdds.Serialization.Processors;

	public static class Serializer
	{
		/// <summary>
		/// Attempt to serialize the object based on the defined serialziation processors found in the definition.
		/// </summary>
		/// <param name="objectToSerialize">Object to serialize.</param>
		/// <param name="serializationDefinition">Serialization definition to guide the serialization process.</param>
		/// <returns>A serialized representation of the object.</returns>
		public static object Serialize(object objectToSerialize, ISerializationDefinition serializationDefinition)
		{
			serializationDefinition.ThrowIfNull(nameof(serializationDefinition));

			if (serializationDefinition.SerializationProcessors == null)
			{
				throw new SerializationException(string.Format("No serialization processors defined in the definition of type {0}.", serializationDefinition.GetType().Name));
			}

			object result = null;
			foreach (ISerializationProcessor processor in serializationDefinition.SerializationProcessors)
			{
				if (processor.Serialize(objectToSerialize, out result))
				{
					return result;
				}
			}

			if (objectToSerialize == null)
			{
				throw new SerializationException("Failed to process null value.");
			}
			else
			{
				throw new SerializationException(string.Format("Failed to process value of type {0}.", objectToSerialize.GetType().Name));
			}
		}

		/// <summary>
		/// Deserialize the data into a new instance of the target type based on the deserialization processors found in the definition.
		/// </summary>
		/// <param name="dataToDeserialize">Data to apply to the instance.</param>
		/// <param name="deserializationDefinition">Serialization definition to guide the deserialization process.</param>
		/// <typeparam name="TTarget">Type of deserialized instance.</typeparam>
		/// <returns>An instance of the target type with the data applied onto it.</returns>
		public static TTarget Deserialize<TTarget>(object dataToDeserialize, ISerializationDefinition deserializationDefinition)
		{
			return (TTarget)Deserialize(typeof(TTarget), dataToDeserialize, deserializationDefinition);
		}

		/// <summary>
		/// Deserialize the data into a new instance of the target type based on the deserialization processors found in the definition.
		/// </summary>
		/// <param name="targetType">Type of deserialized instance.</param>
		/// <param name="dataToDeserialize">Data to apply to the instance.</param>
		/// <param name="deserializationDefinition">Serialization definition to guide the deserialization process.</param>
		/// <returns>An instance of the target type with the data applied onto it.</returns>
		public static object Deserialize(Type targetType, object dataToDeserialize, ISerializationDefinition deserializationDefinition)
		{
			targetType.ThrowIfNull(nameof(targetType));
			deserializationDefinition.ThrowIfNull(nameof(deserializationDefinition));

			if (deserializationDefinition.DeserializationProcessors == null)
			{
				throw new SerializationException("No deserialization processors are defined in the definition of type {0}.", deserializationDefinition.GetType().Name);
			}

			object result = null;
			foreach (IDeserializationProcessor processor in deserializationDefinition.DeserializationProcessors)
			{
				if (processor.Deserialize(targetType, dataToDeserialize, out result))
				{
					return result;
				}
			}

			throw new SerializationException(string.Format("Failed to deserialize a source value to target type {0}.", targetType.Name));
		}

		/// <summary>
		/// Deserialize the data onto an exiting object based on the deserialization processors found in the definition.
		/// </summary>
		/// <param name="deserializationTarget">Object onto which the data is applied.</param>
		/// <param name="dataToDeserialize">Data to apply to the target.</param>
		/// <param name="deserializationDefinition">Serialization definition to guide the deserialization process.</param>
		public static void Deserialize(object deserializationTarget, object dataToDeserialize, ISerializationDefinition deserializationDefinition)
		{
			deserializationTarget.ThrowIfNull(nameof(deserializationTarget));
			deserializationDefinition.ThrowIfNull(nameof(deserializationTarget));

			foreach (IDeserializationProcessor processor in deserializationDefinition.DeserializationProcessors)
			{
				if ((processor == null) || !(processor is IDeserializationToTargetProcessor))
				{
					continue;
				}

				// If the processor implements this interface, and was able to successfully map, then we can stop.
				if ((processor as IDeserializationToTargetProcessor).Deserialize(deserializationTarget, dataToDeserialize))
				{
					return;
				}
			}

			throw new SerializationException(string.Format("Failed to deserialize a source value to a target instance of type {0}.", deserializationTarget.GetType().Name));
		}
	}
}
