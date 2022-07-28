namespace ImpossibleOdds.Serialization
{
	using System;
	using System.Collections;
	// using System.Collections.Generic;
	using System.Collections.Concurrent;
	using System.Runtime.Serialization;
	using ImpossibleOdds.Serialization.Caching;

	public static class SerializationUtilities
	{
		private static readonly ConcurrentDictionary<Type, object> defaultValueCache = new ConcurrentDictionary<Type, object>();
		private static readonly ConcurrentDictionary<Type, LookupCollectionTypeInfo> lookupTypeInfoCache = new ConcurrentDictionary<Type, LookupCollectionTypeInfo>();
		private static readonly ConcurrentDictionary<Type, SequenceCollectionTypeInfo> sequenceTypeInfoCache = new ConcurrentDictionary<Type, SequenceCollectionTypeInfo>();
		private static readonly ConcurrentDictionary<Type, ISerializationReflectionMap> typeMapCache = new ConcurrentDictionary<Type, ISerializationReflectionMap>();

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
		/// Gets The default value for a type.
		/// </summary>
		/// <param name="type">The type for which to retrieve the default value.</param>
		/// <returns>The default value for the type. This will be null for non-value types.</returns>
		public static object GetDefaultValue(Type type)
		{
			if (type.IsValueType)
			{
				return defaultValueCache.ContainsKey(type) ? defaultValueCache[type] : defaultValueCache.GetOrAdd(type, Activator.CreateInstance(type));
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Creates a new (uninitialized) instance of the requested type.
		/// </summary>
		/// <returns>Instance of the requested type.</returns>
		public static object CreateInstance(Type instanceType)
		{
			if (instanceType.IsValueType)
			{
				return Activator.CreateInstance(instanceType, true);
			}
			else
			{
				if (instanceType.IsInterface)
				{
					throw new SerializationException("Cannot create instane of type {0} because it is an interface.", instanceType.Name);
				}
				else if (instanceType.IsAbstract)
				{
					throw new SerializationException("Cannot create instance of type {0} because it is abstract.", instanceType.Name);
				}

				return FormatterServices.GetUninitializedObject(instanceType);
			}
		}

		/// <summary>
		/// Explicitly converts a value to a certain type. A value may have been processed by a processor, but may still need a final conversion to the final type.
		/// </summary>
		/// <param name="value">The value to be converted to the target type.</param>
		/// <typeparam name="TTarget">The target type to which the object should be converted.</typeparam>
		/// <returns>An instance of the objected in the target type.</returns>
		public static TTarget PostProcessValue<TTarget>(object value)
		{
			return (TTarget)PostProcessValue(value, typeof(TTarget));
		}

		/// <summary>
		/// Explicitly converts a value to a certain type. A value may have been processed by a processor, but may still need a final conversion to the final type.
		/// </summary>
		/// <param name="value">The value to be converted to the target type.</param>
		/// <param name="targetType">The target type to which the object should be converted.</param>
		/// <returns>An instance of the objected in the target type.</returns>
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
			return
				((value == null) && IsNullableType(elementType)) ||
				((value != null) && elementType.IsAssignableFrom(value.GetType()));
		}

		/// <summary>
		/// Insert a value in the sequence at the defined index.
		/// Depending on the type of the collection, it may alter its size to insert the value at the requested index.
		/// The value will be processed to a compatible element type if the collection enforces one.
		/// </summary>
		/// <param name="collection">The collection into which the value should be inserted.</param>
		/// <param name="collectionInfo">Type information about the collection.</param>
		/// <param name="index">Index at which the value should be inserted.</param>
		/// <param name="value">The value to be inserted.</param>
		public static void InsertInSequence(IList collection, SequenceCollectionTypeInfo collectionInfo, int index, object value)
		{
			value = collectionInfo.PostProcessValue(value);

			if (collection.IsReadOnly)
			{
				throw new SerializationException("The collection of type {0} is read-only. No elements can be inserted.", collection.GetType().Name);
			}

			// Arrays are treated differently compared to lists.
			if (collectionInfo.isArray && (collection is Array array))
			{
				if (index >= array.Length)
				{
					throw new SerializationException();
				}

				array.SetValue(value, index);
			}
			else
			{
				if (index >= collection.Count)
				{
					if (collection.IsFixedSize)
					{
						throw new SerializationException("The collection of type {0} has a fixed size ({1}). An element was requested to be inserted at index {2}.", collection.GetType().Name, collection.Count, index);
					}

					// Grow the collection until the value can be inserted.
					object defaultValue = SerializationUtilities.GetDefaultValue(collectionInfo.elementType);
					do
					{
						collection.Add(defaultValue);
					} while (index >= collection.Count);
				}

				collection[index] = value;
			}
		}

		/// <summary>
		/// Insert a key and value in the lookup structure.
		/// The key and value will be processed to a compatible key or value type if the collection enforces those.
		/// </summary>
		/// <param name="collection">The collection into which the key and value should be inserted.</param>
		/// <param name="collectionInfo">Type information about the collection.</param>
		/// <param name="key">The key for the value that will be inserted.</param>
		/// <param name="value">The value that will be inserted, associated with the key.</param>
		public static void InsertInLookup(IDictionary collection, LookupCollectionTypeInfo collectionInfo, object key, object value)
		{
			key = collectionInfo.PostProcessKey(key);
			value = collectionInfo.PostProcessValue(value);

			if (collection.IsReadOnly)
			{
				throw new SerializationException("The collection of type {0} is read-only. No elements can be inserted.", collection.GetType().Name);
			}
			else if (collection.IsFixedSize)
			{
				throw new SerializationException("The collection of type {0} has a fixed size. No elements can be inserted.", collection.GetType().Name);
			}
			else
			{
				collection[key] = value;
			}
		}

		/// <summary>
		/// Get additional information about the collection's type, i.e. whether it is constraint by generic parameters.
		/// </summary>
		/// <param name="instance">The instance for which to fetch additional information.</param>
		/// <returns>Information about the collection's type.</returns>
		public static LookupCollectionTypeInfo GetCollectionTypeInfo(IDictionary instance)
		{
			instance.ThrowIfNull(nameof(instance));
			return lookupTypeInfoCache.GetOrAdd(instance.GetType(), (type) => new LookupCollectionTypeInfo(type));
		}

		/// <summary>
		/// Get additional information about the collection's type, i.e. whether it is constraint by generic parameters.
		/// </summary>
		/// <param name="instance">The instance for which to fetch additional information.</param>
		/// <returns>Information about the collection's type.</returns>
		public static SequenceCollectionTypeInfo GetCollectionTypeInfo(IList instance)
		{
			instance.ThrowIfNull(nameof(instance));
			return sequenceTypeInfoCache.GetOrAdd(instance.GetType(), (type) => new SequenceCollectionTypeInfo(type));
		}

		/// <summary>
		/// Retrieve the cached information about a type.
		/// </summary>
		/// <param name="type">The type for which to retrieve the cached information.</param>
		/// <returns>The type cache associated with the given type.</returns>
		public static ISerializationReflectionMap GetTypeMap(Type target)
		{
			target.ThrowIfNull(nameof(target));
			return typeMapCache.GetOrAdd(target, (type) => new SerializationReflectionMap(type));
		}
	}
}
