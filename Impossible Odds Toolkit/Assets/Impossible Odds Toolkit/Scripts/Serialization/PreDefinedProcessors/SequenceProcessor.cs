namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// A (de)serialization processor for list-like data structures.
	/// </summary>
	public class SequenceProcessor : AbstractCollectionProcessor, ISerializationProcessor, IDeserializationToTargetProcessor
	{
		private new IIndexSerializationDefinition definition;

		public SequenceProcessor(IIndexSerializationDefinition definition, bool strictTypeChecking = DefaultOptions.StrictTypeChecking)
		: base(definition, strictTypeChecking)
		{
			this.definition = definition;
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

			if ((objectToSerialize is string) || !typeof(IEnumerable).IsAssignableFrom(objectToSerialize.GetType()))
			{
				serializedResult = null;
				return false;
			}

			IEnumerable sourceValues = (IEnumerable)objectToSerialize;
			int sourceCount = CountElements(sourceValues);

			// Depending on whether the given type is an array or another type of list, we need to treat element insertion differently.
			if (definition.IndexBasedDataType.IsArray)
			{
				Type elementType = definition.IndexBasedDataType.GetElementType();
				bool isTypeConstrained = (elementType != typeof(object));
				Array collection = Array.CreateInstance(elementType, sourceCount);

				int i = 0;
				IEnumerator it = sourceValues.GetEnumerator();
				while (it.MoveNext())
				{
					object processedValue = Serializer.Serialize(it.Current, definition);
					if (!isTypeConstrained || PassesTypeRestriction(processedValue, elementType))
					{
						collection.SetValue(processedValue, i);
					}

					++i;
				}

				serializedResult = collection;
			}
			else
			{
				IList collection = Activator.CreateInstance(definition.IndexBasedDataType, true) as IList;
				Type genericType = SerializationUtilities.GetGenericType(definition.IndexBasedDataType, typeof(IList<>));
				Type genericParam = (genericType != null) ? genericType.GetGenericArguments()[0] : null;
				bool isTypeConstrained = (genericParam != null) && (genericParam != typeof(object));

				int i = 0;
				IEnumerator it = sourceValues.GetEnumerator();
				while (it.MoveNext())
				{
					object processedValue = Serializer.Serialize(it.Current, definition);
					if (!isTypeConstrained || PassesTypeRestriction(processedValue, genericParam))
					{
						collection.Add(processedValue);
					}

					++i;
				}

				serializedResult = collection;
			}

			return true;
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
			// Check if the target implements the general IList interface, if not, we can just skip it altogether.
			if ((targetType == null) || !typeof(IList).IsAssignableFrom(targetType))
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
			else if (!typeof(IList).IsAssignableFrom(dataToDeserialize.GetType()))
			{
				throw new SerializationException(string.Format("The source value is expected to implement the {0} interface to process to target type {1}.", typeof(IList).Name, targetType.Name));
			}

			// Depending on whether the target type is an array or another type of list, we need to treat element insertion differently.
			IList targetCollection;
			if (targetType.IsArray)
			{
				IList sourceValues = dataToDeserialize as IList;
				targetCollection = Array.CreateInstance(targetType.GetElementType(), sourceValues.Count);
			}
			else
			{
				targetCollection = Activator.CreateInstance(targetType, true) as IList;
			}

			if (!Deserialize(targetCollection, dataToDeserialize))
			{
				throw new SerializationException(string.Format("Unexpected failure to process source value of type {0} to target collection of type {1}.", dataToDeserialize.GetType().Name, targetType.Name));
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

			if (!typeof(IList).IsAssignableFrom(dataToDeserialize.GetType()))
			{
				throw new SerializationException(string.Format("The source value is expected to implement the {0} interface to process to target instance of type {1}.", typeof(IList).Name, deserializationTarget.GetType().Name));
			}

			Type targetType = deserializationTarget.GetType();
			IList sourceValues = dataToDeserialize as IList;

			// There's a distinction between lists and arrays...
			if (targetType.IsArray)
			{
				Type elementType = targetType.GetElementType();
				bool isTypeConstrained = (elementType != typeof(object));
				Array targetCollection = deserializationTarget as Array;

				for (int i = 0; i < sourceValues.Count; ++i)
				{
					if (i < targetCollection.Length)
					{
						object processedValue = Serializer.Deserialize(elementType, sourceValues[i], definition);
						processedValue = SerializationUtilities.PostProcessRequestValue(processedValue, elementType);

						if (!isTypeConstrained || PassesTypeRestriction(processedValue, elementType))
						{
							targetCollection.SetValue(processedValue, i);
						}
					}
					else
					{
						Debug.Warning("The target array is shorter than the source collection. Excess values have not been processed.");
						break;
					}
				}
			}
			else
			{
				// Check whether the target implements a generic variant and if the arguments apply additional type restrictions.
				IList targetCollection = deserializationTarget as IList;
				targetCollection.Clear();

				Type genericType = SerializationUtilities.GetGenericType(targetType, typeof(List<>));
				Type genericParam = (genericType != null) ? genericType.GetGenericArguments()[0] : null;
				bool isTypeConstrained = (genericParam != null) && (genericParam != typeof(object));

				Type elementType = isTypeConstrained ? genericParam : typeof(object);

				for (int i = 0; i < sourceValues.Count; ++i)
				{
					object processedValue = Serializer.Deserialize(elementType, sourceValues[i], definition);
					processedValue = SerializationUtilities.PostProcessRequestValue(processedValue, elementType);

					if (!isTypeConstrained || PassesTypeRestriction(processedValue, elementType))
					{
						targetCollection.Add(processedValue);
					}
					else
					{
						if (sourceValues[i] == null)
						{
							Debug.Warning("A null value could not be processed to a valid value for target of type {0}. Skipping value.", targetType.Name);
						}
						else
						{
							Debug.Warning("A value of type {0} could not be processed to a valid value for target of type {1}. Skipping value.", sourceValues[i].GetType().Name, targetType.Name);
						}
					}
				}
			}

			return true;
		}
	}
}
