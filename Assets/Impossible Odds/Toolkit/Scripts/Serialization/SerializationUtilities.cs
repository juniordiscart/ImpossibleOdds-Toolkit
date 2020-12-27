namespace ImpossibleOdds.Serialization
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public static class SerializationUtilities
	{
		private static readonly Dictionary<Type, LookupCollectionTypeInfo> lookupTypeInfoCache = new Dictionary<Type, LookupCollectionTypeInfo>();
		private static readonly Dictionary<Type, SequenceCollectionTypeInfo> sequenceTypeInfoCache = new Dictionary<Type, SequenceCollectionTypeInfo>();

		/// <summary>
		/// Test wether a type is truely nullable.
		/// </summary>
		/// <param name="type">Type to test.</param>
		/// <returns>True if the type is nullable, false otherwise.</returns>
		public static bool IsNullableType(Type type)
		{
			type.ThrowIfNull(nameof(type));
			return !type.IsValueType || (Nullable.GetUnderlyingType(type) != null);
		}

		/// <summary>
		/// Checks whether the given type is assignable to its generic counterpart.
		/// </summary>
		/// <param name="givenType">The type to be tested.</param>
		/// <param name="genericTypeDefinition">The generic type to be tested against.</param>
		public static bool IsAssignableToGenericType(Type givenType, Type genericTypeDefinition)
		{
			return GetGenericType(givenType, genericTypeDefinition) != null;
		}

		/// <summary>
		/// Finds the first occurence of the generic type that matches the given generic type definition. Null is returned if none could be found.
		/// From: https://stackoverflow.com/a/1075059
		/// </summary>
		/// <param name="givenType">The type to get the generic variant of.</param>
		/// <param name="genericTypeDefinition">The generic type to find.</param>
		public static Type GetGenericType(Type givenType, Type genericTypeDefinition)
		{
			if (!genericTypeDefinition.IsGenericTypeDefinition)
			{
				throw new ArgumentException("The given type is not a generic type definition.");
			}

			if (givenType.IsGenericType && (givenType.GetGenericTypeDefinition() == genericTypeDefinition))
			{
				return givenType;
			}

			Type[] interfaceTypes = givenType.GetInterfaces();
			foreach (Type it in interfaceTypes)
			{
				if (it.IsGenericType && (it.GetGenericTypeDefinition() == genericTypeDefinition))
				{
					return it;
				}
			}

			Type baseType = givenType.BaseType;
			if (baseType == null)
			{
				return null;
			}

			return GetGenericType(baseType, genericTypeDefinition);
		}

		/// <summary>
		/// Explicitly converts a value to a certain type. A value may have been processed by a processor, but may still need a final conversion to the final type.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		public static object PostProcessValue(object value, Type targetType)
		{
			targetType.ThrowIfNull(nameof(targetType));

			if ((value == null) || targetType.IsAssignableFrom(value.GetType()))
			{
				return value;
			}

			if (typeof(IConvertible).IsAssignableFrom(targetType))
			{
				// Try to convert the value to the target type
				return Convert.ChangeType(value, targetType);
			}
			else
			{
				Log.Warning("Target type {0} does not implement the {1} interface to post-process a value of type {2}.", targetType.Name, typeof(IConvertible).Name, value.GetType().Name);
				return value;
			}
		}

		/// <summary>
		/// Checks whether the given value can be assigned to the given type and checks whether the type is nullable.
		/// </summary>
		/// <param name="value">The value to test against the type.</param>
		/// <param name="elementType">The type to check against.</param>
		/// <returns>True if the value can be assigned, false otherwise.</returns>
		public static bool PassesElementTypeRestriction(object value, Type elementType)
		{
			bool isNullable = IsNullableType(elementType);
			if ((value == null) && !isNullable)
			{
				return false;
			}
			else if ((value != null) && !elementType.IsAssignableFrom(value.GetType()))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Insert every element from the source values into the target collection.
		/// Each value is given a post processing step to match the element type of the target collection.
		/// </summary>
		/// <param name="sourceValues">The source values to insert in the target collection.</param>
		/// <param name="targetCollection">The target collection in which to insert the source values.</param>
		public static void FillSequence(IEnumerable sourceValues, IList targetCollection)
		{
			sourceValues.ThrowIfNull(nameof(sourceValues));
			targetCollection.ThrowIfNull(nameof(targetCollection));
			SequenceCollectionTypeInfo collectionTypeInfo = GetCollectionTypeInfo(targetCollection);

			int i = 0;
			IEnumerator it = sourceValues.GetEnumerator();
			while (it.MoveNext())
			{
				object currentValue = it.Current;
				if (collectionTypeInfo.isTypeConstrained && !PassesElementTypeRestriction(currentValue, collectionTypeInfo.elementType))
				{
					currentValue = PostProcessValue(currentValue, collectionTypeInfo.elementType);
				}

				if (collectionTypeInfo.isArray)
				{
					(targetCollection as Array).SetValue(currentValue, i);
				}
				else
				{
					targetCollection.Add(currentValue);
				}

				++i;
			}
		}

		/// <summary>
		/// Insert every key-value pair found in the source values into the target collection.
		/// Each key and value is given a post processing step to match the key and value types of the target collection.
		/// </summary>
		/// <param name="sourceValues">The source values to insert in the target collection.</param>
		/// <param name="targetCollection">The target collection in which to transfer the key-value pairs from the source values.</param>
		public static void FillLookup(IDictionary sourceValues, IDictionary targetCollection)
		{
			sourceValues.ThrowIfNull(nameof(sourceValues));
			targetCollection.ThrowIfNull(nameof(targetCollection));
			LookupCollectionTypeInfo collectionTypeInfo = GetCollectionTypeInfo(targetCollection);

			IDictionaryEnumerator it = sourceValues.GetEnumerator();
			while (it.MoveNext())
			{
				object currentKey = it.Key;
				if (collectionTypeInfo.isKeyTypeConstrained && !PassesElementTypeRestriction(currentKey, collectionTypeInfo.keyType))
				{
					currentKey = PostProcessValue(currentKey, collectionTypeInfo.keyType);
				}

				object currentValue = it.Value;
				if (collectionTypeInfo.isValueTypeConstrained && !PassesElementTypeRestriction(currentValue, collectionTypeInfo.valueType))
				{
					currentValue = PostProcessValue(currentValue, collectionTypeInfo.valueType);
				}

				targetCollection.Add(currentKey, currentValue);
			}
		}

		private static LookupCollectionTypeInfo GetCollectionTypeInfo(IDictionary instance)
		{
			instance.ThrowIfNull(nameof(instance));

			Type instanceType = instance.GetType();
			if (!lookupTypeInfoCache.ContainsKey(instanceType))
			{
				lookupTypeInfoCache[instanceType] = new LookupCollectionTypeInfo(instance);
			}

			return lookupTypeInfoCache[instanceType];
		}

		private static SequenceCollectionTypeInfo GetCollectionTypeInfo(IList instance)
		{
			instance.ThrowIfNull(nameof(instance));

			Type instanceType = instance.GetType();
			if (!sequenceTypeInfoCache.ContainsKey(instanceType))
			{
				sequenceTypeInfoCache[instanceType] = new SequenceCollectionTypeInfo(instance);
			}

			return sequenceTypeInfoCache[instanceType];
		}
	}
}
