namespace ImpossibleOdds.DataMapping.Processors
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// A mapping processor to map sequences, i.e. array, list, sets, ... Each element of the sequence is processed individually as well.
	/// </summary>
	public class SequenceMappingProcessor : AbstractCollectionMappingProcessor, IMapToDataStructureProcessor, IMapFromDataStructureToTargetProcessor
	{
		private new IIndexMappingDefinition definition;

		public SequenceMappingProcessor(IIndexMappingDefinition definition, bool strictTypeChecking = DefaultOptions.StrictTypeChecking)
		: base(definition, strictTypeChecking)
		{
			this.definition = definition;
		}

		/// <summary>
		/// Attempts to map the source value to another sequence in which all individual elements are processed by the data mapper.
		/// </summary>
		/// <param name="sourceValue">The source value to map to another sequence. The processor accepts the source value if it is also a sequence-like data structure.</param>
		/// <param name="objResult">The result in which the transformed sequence is stored.</param>
		/// <returns>True if the mapping is accepted for processing, false otherwise.</returns>
		public bool MapToDataStructure(object sourceValue, out object objResult)
		{
			// Accept null values.
			if ((sourceValue == null))
			{
				objResult = null;
				return true;
			}

			if ((sourceValue is string) || !typeof(IEnumerable).IsAssignableFrom(sourceValue.GetType()))
			{
				objResult = null;
				return false;
			}

			IEnumerable sourceValues = (IEnumerable)sourceValue;

			int sourceCount = CountElements(sourceValues);

			// Depending on whether the given type is an array or another type of list, we need to treat element insertion differently.
			if (definition.IndexBasedMapType.IsArray)
			{
				Type elementType = definition.IndexBasedMapType.GetElementType();
				bool isTypeConstrained = (elementType != typeof(object));
				Array collection = Array.CreateInstance(elementType, sourceCount);

				int i = 0;
				IEnumerator it = sourceValues.GetEnumerator();
				while (it.MoveNext())
				{
					object processedValue = DataMapper.MapToDataStructure(it.Current, definition);
					if (!isTypeConstrained || PassesTypeRestriction(processedValue, elementType))
					{
						collection.SetValue(processedValue, i);
					}

					++i;
				}

				objResult = collection;
			}
			else
			{
				IList collection = Activator.CreateInstance(definition.IndexBasedMapType, true) as IList;
				Type genericType = DataMappingUtilities.GetGenericType(definition.IndexBasedMapType, typeof(IList<>));
				Type genericParam = (genericType != null) ? genericType.GetGenericArguments()[0] : null;
				bool isTypeConstrained = (genericParam != null) && (genericParam != typeof(object));

				int i = 0;
				IEnumerator it = sourceValues.GetEnumerator();
				while (it.MoveNext())
				{
					object processedValue = DataMapper.MapToDataStructure(it.Current, definition);
					if (!isTypeConstrained || PassesTypeRestriction(processedValue, genericParam))
					{
						collection.Add(processedValue);
					}

					++i;
				}

				objResult = collection;
			}

			return true;
		}

		/// <summary>
		/// Attempts to map the object to process to a sequence-like data structure, only if the this object is also a sequence-like object. Each individual element is processed.
		/// </summary>
		/// <param name="targetType">The expected type of the returned sequence, if accepted.</param>
		/// <param name="objToProcess">The sequence-like data structure to process.</param>
		/// <param name="objResult">The result in which the mapped data structured is stored.</param>
		/// <returns>True if the mapping is accepted for processing, false otherwise.</returns>
		public bool MapFromDataStructure(Type targetType, object objToProcess, out object objResult)
		{
			// Check if the target implements the general IList interface, if not, we can just skip it altogether.
			if ((targetType == null) || !typeof(IList).IsAssignableFrom(targetType))
			{
				objResult = null;
				return false;
			}

			// If the value is null, we can just assign it.
			if (objToProcess == null)
			{
				objResult = null;
				return true;
			}

			if (targetType.IsAbstract || targetType.IsInterface)
			{
				throw new DataMappingException(string.Format("Target type {0} is abstract or an interface. Cannot create an instance to process the source values.", targetType.Name));
			}
			else if (!typeof(IList).IsAssignableFrom(objToProcess.GetType()))
			{
				throw new DataMappingException(string.Format("The source value is expected to implement the {0} interface to process to target type {1}.", typeof(IList).Name, targetType.Name));
			}

			// Depending on whether the target type is an array or another type of list, we need to treat element insertion differently.
			IList targetCollection;
			if (targetType.IsArray)
			{
				IList sourceValues = objToProcess as IList;
				targetCollection = Array.CreateInstance(targetType.GetElementType(), sourceValues.Count);
			}
			else
			{
				targetCollection = Activator.CreateInstance(targetType, true) as IList;
			}

			if (!MapFromDataStructure(targetCollection, objToProcess))
			{
				throw new DataMappingException(string.Format("Unexpected failure to process source value of type {0} to target collection of type {1}.", objToProcess.GetType().Name, targetType.Name));
			}

			objResult = targetCollection;
			return true;
		}

		/// <summary>
		/// Attempts to map the object to process to the instance of the target, only if both are sequence-like data structures. Each element is processed individually.
		/// </summary>
		/// <param name="target">The target object in which to store the result.</param>
		/// <param name="objToProcess">The object to map to the target.</param>
		/// <returns>True if the mapping is accepted for processing, false otherwise.</returns>
		public bool MapFromDataStructure(object target, object objToProcess)
		{
			if ((target == null) || !(target is IList))
			{
				return false;
			}

			// If there is nothing to do...
			if (objToProcess == null)
			{
				return true;
			}

			if (!typeof(IList).IsAssignableFrom(objToProcess.GetType()))
			{
				throw new DataMappingException(string.Format("The source value is expected to implement the {0} interface to process to target instance of type {1}.", typeof(IList).Name, target.GetType().Name));
			}

			Type targetType = target.GetType();
			IList sourceValues = objToProcess as IList;

			if (targetType.IsArray)
			{
				Type elementType = targetType.GetElementType();
				bool isTypeConstrained = (elementType != typeof(object));
				Array targetCollection = target as Array;

				for (int i = 0; i < sourceValues.Count; ++i)
				{
					if (i < targetCollection.Length)
					{
						object processedValue = DataMapper.MapFromDataStructure(elementType, sourceValues[i], definition);
						processedValue = DataMappingUtilities.PostProcessRequestValue(processedValue, elementType);

						if (!isTypeConstrained || PassesTypeRestriction(processedValue, elementType))
						{
							targetCollection.SetValue(processedValue, i);
						}
					}
					else
					{
#if IMPOSSIBLE_ODDS_VERBOSE
						Debug.LogWarningFormat("The target array is shorter than the source collection. Excess values have not been processed.");
#endif
						break;
					}
				}
			}
			else
			{
				// Check whether the target implements a generic variant and if the arguments apply additional type restrictions.
				IList targetCollection = target as IList;
				targetCollection.Clear();

				Type genericType = DataMappingUtilities.GetGenericType(targetType, typeof(List<>));
				Type genericParam = (genericType != null) ? genericType.GetGenericArguments()[0] : null;
				bool isTypeConstrained = (genericParam != null) && (genericParam != typeof(object));

				Type elementType = isTypeConstrained ? genericParam : typeof(object);

				for (int i = 0; i < sourceValues.Count; ++i)
				{
					object processedValue = DataMapper.MapFromDataStructure(elementType, sourceValues[i], definition);
					processedValue = DataMappingUtilities.PostProcessRequestValue(processedValue, elementType);

					if (!isTypeConstrained || PassesTypeRestriction(processedValue, elementType))
					{
						targetCollection.Add(processedValue);
					}
#if IMPOSSIBLE_ODDS_VERBOSE
					else
					{
						if (sourceValues[i] == null)
						{
							Debug.LogWarningFormat("A null value could not be processed to a valid value for target of type {0}. Skipping value.", targetType.Name);
						}
						else
						{
							Debug.LogWarningFormat("A value of type {0} could not be processed to a valid value for target of type {1}. Skipping value.", sourceValues[i].GetType().Name, targetType.Name);
						}
					}
#endif
				}
			}

			return true;
		}
	}
}
