﻿namespace ImpossibleOdds.DataMapping.Processors
{
	using System;
	using System.Reflection;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using UnityEngine;

	public abstract class AbstractCustomObjectProcessor : AbstractMappingProcessor
	{
		/// <summary>
		/// Cache of attributes, organized by attribute type.
		/// </summary>
		/// <typeparam name="Type">Type of the attribute.</typeparam>
		/// <typeparam name="AttributeFieldsCache">Cache of types with the attributes defined in them.</typeparam>
		protected static Dictionary<Type, AttributeFieldsCache> attributeCache = new Dictionary<Type, AttributeFieldsCache>();

		/// <summary>
		/// Cache of
		/// </summary>
		/// <typeparam name="Type"></typeparam>
		/// <typeparam name="TypeResolveCache"></typeparam>
		/// <returns></returns>
		protected static Dictionary<Type, TypeResolveCache> typeResolveCache = new Dictionary<Type, TypeResolveCache>();

		public AbstractCustomObjectProcessor(IMappingDefinition definition)
		: base(definition)
		{ }

		/// <summary>
		/// Fetch all attributes defined on the target type.
		/// </summary>
		/// <param name="targetType">The class type of which to fetch the attributes that are defined on its fields.</param>
		/// <param name="attributeType">The attribute type to look for that is defined on the target type's fields.</param>
		/// <returns></returns>
		protected static ReadOnlyCollection<FieldAtrributeTuple> GetAttributeFields(Type targetType, Type attributeType)
		{
			if (!attributeCache.ContainsKey(attributeType))
			{
				attributeCache.Add(attributeType, new AttributeFieldsCache());
			}

			AttributeFieldsCache attributesCache = attributeCache[attributeType];
			if (attributesCache.ContainsKey(targetType))
			{
				return attributesCache[targetType].AsReadOnly();
			}

			// Collection in which we will store the cached fields.
			List<FieldAtrributeTuple> targetFields = new List<FieldAtrributeTuple>();
			attributesCache.Add(targetType, targetFields);

			// Fetch all fields across the type hierarchy.
			while ((targetType != null) && (targetType != typeof(object)))
			{
				FieldInfo[] fields = targetType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (FieldInfo field in fields)
				{
					if (field.DeclaringType != targetType)
					{
						continue;
					}

					Attribute attr = field.GetCustomAttributes(attributeType, false).SingleOrDefault() as Attribute;
					if (attr != null)
					{
						targetFields.Add(new FieldAtrributeTuple(field, attr));
					}
				}

				targetType = targetType.BaseType;
			}

			return targetFields.AsReadOnly();
		}

		/// <summary>
		/// Finds the maximum defined index in the hierarchy.
		/// </summary>
		/// <param name="type">Type of object for which to find the maximum defined index.</param>
		/// <param name="attributeType">Type of the index-based parameter attribute.</param>
		/// <returns>Returns the maximum defined index found. If no index-based parameter attributes were found, -1 is returned.</returns>
		protected static int GetMaxDefinedIndex(Type type, Type attributeType)
		{
			int maxIndex = -1;
			while ((type != null) && (type != typeof(object)))
			{
				ReadOnlyCollection<FieldAtrributeTuple> fields = GetAttributeFields(type, attributeType);
				foreach (FieldAtrributeTuple field in fields)
				{
					maxIndex = Mathf.Max((field.attribute as IIndexParameter).Index, maxIndex);
				}

				type = type.BaseType;
			}

			return maxIndex;
		}

		/// <summary>
		/// Fetch all attributes defined on the target type's class and across its inheritance chain.
		/// </summary>
		/// <param name="targetType">The class type of which to fetch the attributes that are defined on it.</param>
		/// <param name="attributeType">The type of the attribute to look for.</param>
		/// <returns></returns>
		protected static ReadOnlyCollection<Attribute> GetClassTypeResolves(Type targetType, Type attributeType)
		{
			if (!typeResolveCache.ContainsKey(attributeType))
			{
				typeResolveCache.Add(attributeType, new TypeResolveCache());
			}

			TypeResolveCache cache = typeResolveCache[attributeType];
			if (cache.ContainsKey(targetType))
			{
				return cache[targetType].AsReadOnly();
			}

			// Fetch all attributes defined in the inheritance chain.
			List<Attribute> typeResolveAttributes = new List<Attribute>();
			cache.Add(targetType, typeResolveAttributes);
			typeResolveAttributes.AddRange(targetType.GetCustomAttributes(attributeType, true).Cast<Attribute>());
			return typeResolveAttributes.AsReadOnly();
		}

		/// <summary>
		/// Binds an attribute to a field for quick access.
		/// </summary>
		protected class FieldAtrributeTuple
		{
			public readonly FieldInfo field;
			public readonly Attribute attribute;

			public FieldAtrributeTuple(FieldInfo field, Attribute attribute)
			{
				this.field = field;
				this.attribute = attribute;
			}
		}

		/// <summary>
		/// Thin wrapper of a dictionary. The keys are class types. The corresponding values are
		/// a list of all fields that have a certain attribute defined, stored in a tuple containing
		/// the field and the attribute associated with it.
		/// </summary>
		protected class AttributeFieldsCache : Dictionary<Type, List<FieldAtrributeTuple>> { }

		/// <summary>
		/// Thin wrapper of a dictionary. The keys are class/interface types. The corresponding values
		/// are a list of type resolve attributes defined of a certain type.
		/// </summary>
		protected class TypeResolveCache : Dictionary<Type, List<Attribute>> { }
	}
}
