namespace ImpossibleOdds.DataMapping.Processors
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using UnityEngine;

	/// <summary>
	/// Mapping processor to process custom objects to and from a sequence-based data structure.
	/// </summary>
	public class CustomObjectSequenceMappingProcessor : AbstractCustomObjectProcessor, IMapToDataStructureProcessor, IMapFromDataStructureToTargetProcessor
	{
		private new IIndexMappingDefinition definition;

		private bool SupportsTypeResolvement
		{
			get { return definition is IIndexTypeResolve; }
		}

		public CustomObjectSequenceMappingProcessor(IIndexMappingDefinition definition)
		: base(definition)
		{
			this.definition = definition;
		}

		/// <summary>
		/// Tries to map a given source value to a sequence-like data structure according to the defined types in the mapping definition.
		/// </summary>
		/// <param name="sourceValue">The value to be transformed to a sequence-like data structure.</param>
		/// <param name="objResult">Value in which the result will be stored.</param>
		/// <returns>True if mapping is accepted for processing, false otherwise.</returns>
		public bool MapToDataStructure(object sourceValue, out object objResult)
		{
			if (sourceValue == null)
			{
				objResult = sourceValue;
				return true;
			}

			Type sourceType = sourceValue.GetType();
			if (!sourceType.IsDefined(definition.IndexBasedClassMarkingAttribute, true))
			{
				objResult = null;
				return false;
			}

			objResult = ProcessToSequence(sourceType, sourceValue);
			return true;
		}

		/// <summary>
		/// Tries to map a given source value to a custom object of the given type.
		/// </summary>
		/// <param name="targetType">The expected type of the returned instance. This target type will be evaluated for type resolve, if the mapping supports it.</param>
		/// <param name="sourceValue">The value to be mapped from. It is tested whether it adheres to the sequence structure as defined in the mapping definition.</param>
		/// <param name="objResult">Value in which the result will be stored.</param>
		/// <returns>True if the mapping is accepted for processing, false otherwise.</returns>
		public bool MapFromDataStructure(Type targetType, object sourceValue, out object objResult)
		{
			if (targetType == null)
			{
				objResult = null;
				return false;
			}
			else if (sourceValue == null)
			{
				objResult = null;
				return DataMappingUtilities.IsNullableType(targetType);
			}

			object targetInstance = null;
			Type instanceType = ResolveTypeFromSequence(targetType, sourceValue as IList);

			try
			{
				targetInstance = Activator.CreateInstance(instanceType, true);
			}
			catch (System.Exception)
			{
				throw new DataMappingException(string.Format("Failed to create an instance of target type {0}.", targetType.Name));
			}

			if (MapFromDataStructure(targetInstance, sourceValue))
			{
				objResult = targetInstance;
				return true;
			}
			else
			{
				objResult = null;
				return false;
			}
		}

		/// <summary>
		/// Tries to map a given source value unto the target object.
		/// </summary>
		/// <param name="target">The target object unto which the source value is mapped.</param>
		/// <param name="sourceValue">The source value of which the values are mapped from. This source value is expected to be in a sequence-like data structure.</param>
		/// <returns>True if the mapping is accepted for processing, false otherwise.</returns>
		public bool MapFromDataStructure(object target, object sourceValue)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}

			// If the source value is null, then there is little to do.
			if (sourceValue == null)
			{
				return true;
			}
			else if (!target.GetType().IsDefined(definition.IndexBasedClassMarkingAttribute, true))
			{
				return false;
			}

			ProcessFromSequence(target, sourceValue as IList);
			return true;
		}

		private IList ProcessToSequence(Type sourceType, object source)
		{
			// Create the collection in which we will store the values.
			IList values;
			int nrOfElements = GetMaxDefinedIndex(source.GetType(), definition.IndexBasedFieldAttribute);
			nrOfElements += (nrOfElements >= 0) ? 1 : 0;

			// Check whether we want to include type information
			IIndexTypeResolveParameter typeResolveAttr = ResolveTypeToSequence(sourceType);
			if (typeResolveAttr != null)
			{
				nrOfElements = Mathf.Max(typeResolveAttr.Index + 1, nrOfElements);
			}

			// Create the instance of the index-based type. Arrays are a special case.
			GenericTypeInfo genericInfo = new GenericTypeInfo(definition);
			if (definition.IndexBasedMapType.IsArray)
			{
				values = Array.CreateInstance(definition.IndexBasedMapType.GetElementType(), nrOfElements);
			}
			else
			{
				values = Array.CreateInstance(genericInfo.isTypeConstrained ? genericInfo.genericParam : typeof(object), nrOfElements);
				values = Activator.CreateInstance(definition.IndexBasedMapType, new object[] { values }) as IList;
			}

			// Process the marked fields.
			ReadOnlyCollection<FieldAtrributeTuple> sourceFields = GetAttributeFields(sourceType, definition.IndexBasedFieldAttribute);
			foreach (FieldAtrributeTuple sourceField in sourceFields)
			{
				IIndexParameter indexAttribute = sourceField.attribute as IIndexParameter;
				object value = DataMapper.MapToDataStructure(sourceField.field.GetValue(source), definition);
				InsertValueInSequence(sourceType, values, genericInfo, indexAttribute.Index, value);
			}

			// Include the type information, if any was found.
			if (typeResolveAttr != null)
			{
				InsertValueInSequence(sourceType, values, genericInfo, typeResolveAttr.Index, typeResolveAttr.Value);
			}

			return values;
		}

		private void InsertValueInSequence(Type sourceType, IList values, GenericTypeInfo sequenceGenericInfo, int index, object value)
		{
			if ((value != null) && sequenceGenericInfo.isTypeConstrained)
			{
				value = DataMappingUtilities.PostProcessRequestValue(value, sequenceGenericInfo.genericParam);
			}

			if (values[index] != null)
			{
#if IMPOSSIBLE_ODDS_VERBOSE
				Debug.LogWarningFormat("Index {0} for processing an instance of type {1} is used multiple times.", index, sourceType.Name);
#endif
			}

			values[index] = value;
		}

		private void ProcessFromSequence(object target, IList source)
		{
			// Get all of the fields that would like to get their value filled in
			ReadOnlyCollection<FieldAtrributeTuple> targetFields = GetAttributeFields(target.GetType(), definition.IndexBasedFieldAttribute);

			foreach (FieldAtrributeTuple targetField in targetFields)
			{
				IIndexParameter mappingAttribute = targetField.attribute as IIndexParameter;

				// Check whether the source has such an index.
				if (source.Count <= mappingAttribute.Index)
				{
#if IMPOSSIBLE_ODDS_VERBOSE
					Debug.LogWarningFormat("The source does not contain a value at index '{0}' for a target of type {1}.", mappingAttribute.Index, target.GetType().Name);
#endif
					continue;
				}

				object result = DataMapper.MapFromDataStructure(targetField.field.FieldType, source[mappingAttribute.Index], definition);

				if (result == null)
				{
					Type fieldType = targetField.field.FieldType;
					targetField.field.SetValue(target, fieldType.IsValueType ? Activator.CreateInstance(fieldType, true) : null);
				}
				else
				{
					targetField.field.SetValue(target, result);
				}
			}
		}

		private IIndexTypeResolveParameter ResolveTypeToSequence(Type sourceType)
		{
			if (!SupportsTypeResolvement)
			{
				return null;
			}

			IIndexTypeResolve typeResolveImplementation = definition as IIndexTypeResolve;
			IEnumerable<Attribute> typeResolveAttributes = GetClassTypeResolves(sourceType, typeResolveImplementation.TypeResolveAttribute);
			foreach (Attribute attr in typeResolveAttributes)
			{
				IIndexTypeResolveParameter typeResolveAttr = attr as IIndexTypeResolveParameter;
				if (typeResolveAttr == null)
				{
					throw new DataMappingException(string.Format("The attribute of type {0} does not implement the {1} interface and cannot be used for type resolving.", attr.GetType().Name, typeof(IIndexTypeResolveParameter).Name));
				}
				else if (typeResolveAttr.Target == sourceType)
				{
					return typeResolveAttr;
				}
			}

			return null;
		}

		private Type ResolveTypeFromSequence(Type targetType, IList source)
		{
			if (!SupportsTypeResolvement)
			{
				if (targetType.IsAbstract || targetType.IsInterface)
				{
					throw new DataMappingException(string.Format("The target type {0} is abstract or an interface, but no type resolve ({1}) is implemented in mapping definition of type {2}.", targetType.Name, typeof(IIndexTypeResolve).Name, definition.GetType().Name));
				}

				return targetType;
			}

			IIndexTypeResolve typeResolveImplementation = definition as IIndexTypeResolve;
			ReadOnlyCollection<Attribute> typeResolveAttrs = GetClassTypeResolves(targetType, typeResolveImplementation.TypeResolveAttribute);
			foreach (Attribute attr in typeResolveAttrs)
			{
				IIndexTypeResolveParameter typeResolveAttr = attr as IIndexTypeResolveParameter;
				if (typeResolveAttr == null)
				{
					throw new DataMappingException(string.Format("The attribute of type {0} does not implement the {1} interface and cannot be used for type resolving.", attr.GetType().Name, typeof(ILookupTypeResolveParameter).Name));
				}
				else if ((source.Count > typeResolveAttr.Index) && (source[typeResolveAttr.Index] == typeResolveAttr.Value))
				{
					if (targetType.IsAssignableFrom(typeResolveAttr.Target))
					{
						return typeResolveAttr.Target;
					}
					else
					{
						throw new DataMappingException(string.Format("The attribute of type {0}, defined on type {1} or its super types, is matched but cannot be assigned from instance of type {2}.", typeResolveAttr.GetType().Name, targetType.Name, typeResolveAttr.Target.Name));
					}
				}
			}

			return targetType;
		}

		private struct GenericTypeInfo
		{
			public readonly Type genericType;
			public readonly Type genericParam;
			public readonly bool isTypeConstrained;

			public GenericTypeInfo(IIndexMappingDefinition definition)
			{
				genericType = DataMappingUtilities.GetGenericType(definition.IndexBasedMapType, typeof(IList<>));
				genericParam = (genericType != null) ? genericType.GetGenericArguments()[0] : null;
				isTypeConstrained = (genericParam != null) && (genericParam != typeof(object));
			}
		}
	}
}
