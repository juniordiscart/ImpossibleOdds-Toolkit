namespace ImpossibleOdds.DataMapping.Processors
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// A mapping processor to map lookup-like data structures, i.e. dictionaries, ... Each key and value of the lookup is processed individually as well.
	/// </summary>
	public class LookupMappingProcessor : AbstractCollectionMappingProcessor, IMapToDataStructureProcessor, IMapFromDataStructureToTargetProcessor
	{
		private new ILookupMappingDefinition definition;

		public LookupMappingProcessor(ILookupMappingDefinition definition, bool strictTypeChecking = DefaultOptions.StrictTypeChecking)
		: base(definition, strictTypeChecking)
		{
			this.definition = definition;
		}

		/// <summary>
		/// Attempts to map the source value to another lookup in which all key and value pairs are processed by the data mapper.
		/// </summary>
		/// <param name="sourceValue">The source value to map to another lookup. The processor accepts the source value if it is also a lookup-like data structure.</param>
		/// <param name="objResult">The result in which the transformed lookup is stored.</param>
		/// <returns>True if the mapping is accepted for processing, false otherwise.</returns>
		public bool MapToDataStructure(object sourceValue, out object objResult)
		{
			// Accept null values.
			if ((sourceValue == null))
			{
				objResult = null;
				return true;
			}

			if (!typeof(IDictionary).IsAssignableFrom(sourceValue.GetType()))
			{
				objResult = null;
				return false;
			}

			IDictionary sourceValues = (IDictionary)sourceValue;
			IDictionary lookupSet = Activator.CreateInstance(definition.LookupBasedMapType, true) as IDictionary;

			Type genericType = DataMappingUtilities.GetGenericType(definition.LookupBasedMapType, typeof(IDictionary<,>));
			Type[] genericParams = (genericType != null) ? genericType.GetGenericArguments() : null;
			bool isKeyTypeConstrained = (genericParams != null) && (genericParams[0] != typeof(object));
			bool isValueTypeConstrained = (genericParams != null) && (genericParams[1] != typeof(object));

			ICollection keys = sourceValues.Keys;
			foreach (object key in keys)
			{
				object processedKey = DataMapper.MapToDataStructure(key, definition);
				if (isKeyTypeConstrained && !PassesTypeRestriction(processedKey, genericParams[0]))
				{
					continue;
				}

				object processedValue = DataMapper.MapToDataStructure(sourceValues[key], definition);
				if (isValueTypeConstrained && !PassesTypeRestriction(processedValue, genericParams[1]))
				{
					continue;
				}

				lookupSet.Add(processedKey, processedValue);
			}

			objResult = lookupSet;
			return true;
		}

		/// <summary>
		/// Attempts to map the object to process to a lookup-like data structure, only if the this object is also a lookup-like object. Each individual key-value pair is processed.
		/// </summary>
		/// <param name="targetType">The expected type of the returned lookup, if accepted.</param>
		/// <param name="objToProcess">The lookup-like data structure to process.</param>
		/// <param name="objResult">The result in which the mapped data structured is stored.</param>
		/// <returns>True if the mapping is accepted for processing, false otherwise.</returns>
		public bool MapFromDataStructure(Type targetType, object objToProcess, out object objResult)
		{
			// Check if the target implements the general IDictionary interface, if not, we can just skip altogether.
			if ((targetType == null) || !typeof(IDictionary).IsAssignableFrom(targetType))
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
			else if (!typeof(IDictionary).IsAssignableFrom(objToProcess.GetType()))
			{
				throw new DataMappingException(string.Format("The source value is expected to implement the {0} interface to process to target type {1}.", typeof(IDictionary), targetType.Name));
			}

			IDictionary targetCollection = Activator.CreateInstance(targetType, true) as IDictionary;
			if (!MapFromDataStructure(targetCollection, objToProcess))
			{
				throw new DataMappingException(string.Format("Unexpected failure to process source value of type {0} to target collection of type {1}.", objToProcess.GetType().Name, targetType.Name));
			}

			objResult = targetCollection;
			return true;
		}

		/// <summary>
		/// Attempts to map the object to process to the instance of the target, only if both are lookup-like data structures. Each key-value pair is processed individually.
		/// </summary>
		/// <param name="target">The target object in which to store the result.</param>
		/// <param name="objToProcess">The object to map to the target.</param>
		/// <returns>True if the mapping is accepted for processing, false otherwise.</returns>
		public bool MapFromDataStructure(object target, object objToProcess)
		{
			if ((target == null) || !(target is IDictionary))
			{
				return false;
			}

			// If there is nothing to do...
			if (objToProcess == null)
			{
				return true;
			}

			if (!typeof(IDictionary).IsAssignableFrom(objToProcess.GetType()))
			{
				throw new DataMappingException(string.Format("The source value is expected to implement the {0} interface to process to target instance of type {1}.", typeof(IDictionary), target.GetType().Name));
			}

			Type targetType = target.GetType();
			IDictionary targetCollection = target as IDictionary;

			// Check whether the target implements a generic variant and if the arguments apply additional type restrictions.
			Type genericType = DataMappingUtilities.GetGenericType(targetType, typeof(IDictionary<,>));
			Type[] genericParams = (genericType != null) ? genericType.GetGenericArguments() : null;
			bool isKeyTypeConstrained = (genericParams != null) && (genericParams[0] != typeof(object));
			bool isValueTypeConstrained = (genericParams != null) && (genericParams[1] != typeof(object));

			Type keyType = isKeyTypeConstrained ? genericParams[0] : typeof(object);
			Type valueType = isValueTypeConstrained ? genericParams[1] : typeof(object);

			// Process each key-value pair of the source values individually.
			IDictionary sourceValues = objToProcess as IDictionary;
			ICollection keys = sourceValues.Keys;
			foreach (object key in keys)
			{
				object processedKey = DataMapper.MapFromDataStructure(keyType, key, definition);
				processedKey = DataMappingUtilities.PostProcessRequestValue(processedKey, keyType);
				if ((processedKey == null) || (isKeyTypeConstrained && !PassesTypeRestriction(processedKey, keyType)))
				{
#if IMPOSSIBLE_ODDS_VERBOSE
					Debug.LogWarningFormat("A key of type {0} could not be processed to a valid key for target of type {1}. Skipping key and value.", key.GetType(), targetType.Name);
#endif
					continue;
				}

				object processedValue = DataMapper.MapFromDataStructure(valueType, sourceValues[key], definition);
				processedValue = DataMappingUtilities.PostProcessRequestValue(processedValue, valueType);
				if (isValueTypeConstrained && !PassesTypeRestriction(processedValue, valueType))
				{
#if IMPOSSIBLE_ODDS_VERBOSE
					if (sourceValues[key] == null)
					{
						Debug.LogWarningFormat("A null value could not be processed to a valid value for target of type {0}. Skipping key and value.", targetType.Name);
					}
					else
					{
						Debug.LogWarningFormat("A value of type {0} could not be processed to a valid value for target of type {1}. Skipping key and value.", sourceValues[key].GetType().Name, targetType.Name);
					}
#endif
					continue;
				}

				targetCollection.Add(processedKey, processedValue);
			}

			return true;
		}
	}
}
