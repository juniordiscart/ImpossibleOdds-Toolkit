﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace ImpossibleOdds.Serialization.Caching
{
	/// <summary>
	/// Contains type information about the lookup data structure, e.g. whether it restricts its key and/or value to be type restricted.
	/// </summary>
	public readonly struct LookupCollectionTypeInfo
	{
		/// <summary>
		/// Generic type of the lookup data structure.
		/// </summary>
		public readonly Type genericType;
		/// <summary>
		/// The types of the key and value in the lookup data structure.
		/// </summary>
		public readonly Type[] genericParams;
		/// <summary>
		/// The type of keys.
		/// </summary>
		public readonly Type keyType;
		/// <summary>
		/// The type of the values.
		/// </summary>
		public readonly Type valueType;
		/// <summary>
		/// Is the key type constrained?
		/// </summary>
		public readonly bool isKeyTypeConstrained;
		/// <summary>
		/// Is the value type constrained?
		/// </summary>
		public readonly bool isValueTypeConstrained;

		public LookupCollectionTypeInfo(IDictionary instance)
		: this(instance.GetType())
		{ }

		public LookupCollectionTypeInfo(Type collectionType)
		{
			collectionType.ThrowIfNull(nameof(collectionType));
			if (!typeof(IDictionary).IsAssignableFrom(collectionType))
			{
				throw new ArgumentException($"{collectionType.Name} is not a {nameof(IDictionary)}.");
			}

			genericType = SerializationUtilities.GetGenericType(collectionType, typeof(IDictionary<,>));
			genericParams = genericType?.GetGenericArguments();
			keyType = (genericParams != null) ? genericParams[0] : typeof(object);
			valueType = (genericParams != null) ? genericParams[1] : typeof(object);
			isKeyTypeConstrained = (genericParams != null) && (keyType != typeof(object));
			isValueTypeConstrained = (genericParams != null) && (valueType != typeof(object));
		}

		public bool PassesKeyTypeRestriction(object key)
		{
			return !isKeyTypeConstrained || SerializationUtilities.PassesElementTypeRestriction(key, keyType);
		}

		public bool PassesValueTypeRestriction(object value)
		{
			return !isValueTypeConstrained || SerializationUtilities.PassesElementTypeRestriction(value, valueType);
		}

		public object PostProcessKey(object key)
		{
			return !PassesKeyTypeRestriction(key) ? SerializationUtilities.PostProcessValue(key, keyType) : key;
		}

		public object PostProcessValue(object value)
		{
			return !PassesValueTypeRestriction(value) ? SerializationUtilities.PostProcessValue(value, valueType) : value;
		}
	}
}