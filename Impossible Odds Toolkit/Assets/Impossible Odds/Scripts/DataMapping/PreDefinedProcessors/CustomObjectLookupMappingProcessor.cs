namespace ImpossibleOdds.DataMapping.Processors
{
	using ImpossibleOdds;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using UnityEngine;

	/// <summary>
	/// Mapping processor to process custom objects to and from a lookup-based data structure.
	/// </summary>
	public class CustomObjectLookupMappingProcessor : AbstractCustomObjectProcessor, IMapToDataStructureProcessor, IMapFromDataStructureToTargetProcessor
	{
		private new ILookupMappingDefinition definition;

		private bool SupportsTypeResolvement
		{
			get { return definition is ILookupTypeResolve; }
		}

		public CustomObjectLookupMappingProcessor(ILookupMappingDefinition definition)
		: base(definition)
		{
			this.definition = definition;
		}

		/// <summary>
		/// Tries to map a given source value to a lookup data structure according to the defined types in the mapping definition.
		/// </summary>
		/// <param name="sourceValue">The value to be transformed to a lookup data structure.</param>
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
			if (!sourceType.IsDefined(definition.LookupBasedClassMarkingAttribute, true))
			{
				objResult = null;
				return false;
			}

			objResult = ProcessToLookup(sourceType, sourceValue);
			return true;
		}

		/// <summary>
		/// Tries to map a given source value to a custom object of the given type.
		/// </summary>
		/// <param name="targetType">The expected type of the returned instance. This target type will be evaluated for type resolve, if the mapping supports it.</param>
		/// <param name="sourceValue">The value to be mapped from. It is tested whether it adheres to the lookup structure as defined in the mapping definition.</param>
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
			Type instanceType = ResolveTypeFromLookup(targetType, sourceValue as IDictionary);

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
		/// <param name="sourceValue">The source value of which the values are mapped from. This source value is expected to be in a lookup-like data structure.</param>
		/// <returns>True if the mapping is accepted for processing, false otherwise.</returns>
		public bool MapFromDataStructure(object target, object sourceValue)
		{
			target.ThrowIfNull(nameof(target));

			// If the source value is null, then there is little to do.
			if (sourceValue == null)
			{
				return true;
			}
			else if (!target.GetType().IsDefined(definition.LookupBasedClassMarkingAttribute, true))
			{
				return false;
			}

			ProcessFromLookup(target, sourceValue as IDictionary);
			return true;
		}

		private IDictionary ProcessToLookup(Type sourceType, object source)
		{
			IDictionary properties = (IDictionary)Activator.CreateInstance(definition.LookupBasedMapType, false);
			GenericTypeInfo lookupGenericInfo = new GenericTypeInfo(definition);

			ReadOnlyCollection<FieldAtrributeTuple> sourceFields = GetAttributeFields(sourceType, definition.LookupBasedFieldAttribute);
			foreach (FieldAtrributeTuple sourceField in sourceFields)
			{
				object key = GetKey(sourceField);
				object value = sourceField.field.GetValue(source);
				InsertKeyValuePairInLookup(sourceType, properties, lookupGenericInfo, key, value);
			}

			// Include type information, if available.
			ILookupTypeResolveParameter typeResolveAttr = ResolveTypeToLookup(sourceType);
			if (typeResolveAttr != null)
			{
				InsertKeyValuePairInLookup(sourceType, properties, lookupGenericInfo, typeResolveAttr.Key, typeResolveAttr.Value);
			}

			return properties;
		}

		private void InsertKeyValuePairInLookup(Type sourceType, IDictionary properties, GenericTypeInfo lookupGenericInfo, object key, object value)
		{
			// Process the key if type constraints are set.
			if (lookupGenericInfo.isKeyTypeConstrained)
			{
				key = DataMappingUtilities.PostProcessRequestValue(key, lookupGenericInfo.genericParams[0]);
			}

			// Process the value if type constraints are set.
			if ((value != null) && lookupGenericInfo.isValueTypeConstrained)
			{
				value = DataMappingUtilities.PostProcessRequestValue(value, lookupGenericInfo.genericParams[1]);
			}

			if (properties.Contains(key))
			{
				properties[key] = value;
#if IMPOSSIBLE_ODDS_VERBOSE
				Debug.LogWarningFormat("A key with value '{0}' has been defined more than once for source object of type {1}.", key.ToString(), sourceType.Name);
#endif
			}
			else
			{
				properties.Add(key, value);
			}
		}

		private void ProcessFromLookup(object target, IDictionary source)
		{
			// Get all of the fields that would like to get their value filled in
			ReadOnlyCollection<FieldAtrributeTuple> targetFields = GetAttributeFields(target.GetType(), definition.LookupBasedFieldAttribute);

			foreach (FieldAtrributeTuple targetField in targetFields)
			{
				object key = GetKey(targetField);

				// See whether the source contains a value for this field
				if (!source.Contains(key))
				{
					// Check whether this field is marked as required
					if (targetField.field.IsDefined(typeof(RequiredAttribute), false))
					{
						throw new DataMappingException(string.Format("The field {0} is marked as required but is not present in the source.", targetField.field.Name));
					}
					else
					{
#if IMPOSSIBLE_ODDS_VERBOSE
						Debug.LogWarningFormat("The source does not contain a value associated with key '{0}' for a target of type {1}.", key, target.GetType().Name);
#endif
						continue;
					}
				}

				object result = DataMapper.MapFromDataStructure(targetField.field.FieldType, source[key], definition);

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

		private object GetKey(FieldAtrributeTuple field)
		{
			ILookupParameter lookupAttribute = field.attribute as ILookupParameter;
			return (lookupAttribute.Key != null) ? lookupAttribute.Key : field.field.Name;
		}

		private ILookupTypeResolveParameter ResolveTypeToLookup(Type sourceType)
		{
			if (!SupportsTypeResolvement)
			{
				return null;
			}

			ILookupTypeResolve typeResolveImplementation = definition as ILookupTypeResolve;
			IEnumerable<Attribute> typeResolveAttributes = GetClassTypeResolves(sourceType, typeResolveImplementation.TypeResolveAttribute);
			foreach (Attribute attr in typeResolveAttributes)
			{
				ILookupTypeResolveParameter typeResolveAttr = attr as ILookupTypeResolveParameter;
				if (typeResolveAttr == null)
				{
					throw new DataMappingException(string.Format("The attribute of type {0} does not implement the {1} interface and cannot be used for type resolving.", attr.GetType().Name, typeof(ILookupTypeResolveParameter).Name));
				}
				else if (typeResolveAttr.Target == sourceType)
				{
					return typeResolveAttr;
				}
			}

			return null;
		}

		private Type ResolveTypeFromLookup(Type targetType, IDictionary source)
		{
			if (!SupportsTypeResolvement)
			{
				if (targetType.IsAbstract || targetType.IsInterface)
				{
					throw new DataMappingException(string.Format("The target type {0} is abstract or an interface, but no type resolve ({1}) is implemented in mapping definition of type {2}.", targetType.Name, typeof(ILookupTypeResolve).Name, definition.GetType().Name));
				}

				return targetType;
			}

			ILookupTypeResolve typeResolveImplementation = definition as ILookupTypeResolve;
			ReadOnlyCollection<Attribute> typeResolveAttrs = GetClassTypeResolves(targetType, typeResolveImplementation.TypeResolveAttribute);
			foreach (Attribute attr in typeResolveAttrs)
			{
				ILookupTypeResolveParameter typeResolveAttr = attr as ILookupTypeResolveParameter;
				if (typeResolveAttr == null)
				{
					throw new DataMappingException(string.Format("The attribute of type {0} does not implement the {1} interface and cannot be used for type resolving.", attr.GetType().Name, typeof(ILookupTypeResolveParameter).Name));
				}
				else if (source.Contains(typeResolveAttr.Key) && (source[typeResolveAttr.Key] == typeResolveAttr.Value))
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
			public readonly Type[] genericParams;
			public readonly bool isKeyTypeConstrained;
			public readonly bool isValueTypeConstrained;

			public GenericTypeInfo(ILookupMappingDefinition definition)
			{
				genericType = DataMappingUtilities.GetGenericType(definition.LookupBasedMapType, typeof(IDictionary<,>));
				genericParams = (genericType != null) ? genericType.GetGenericArguments() : null;
				isKeyTypeConstrained = (genericParams != null) && (genericParams[0] != typeof(object));
				isValueTypeConstrained = (genericParams != null) && (genericParams[1] != typeof(object));
			}
		}
	}
}
