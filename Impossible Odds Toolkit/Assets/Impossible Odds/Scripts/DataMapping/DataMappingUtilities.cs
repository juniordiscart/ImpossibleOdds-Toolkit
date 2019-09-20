namespace ImpossibleOdds.DataMapping
{
	using ImpossibleOdds;
	using System;

#if IMPOSSIBLE_ODDS_VERBOSE
	using UnityEngine;
#endif

	public static class DataMappingUtilities
	{
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
		/// <returns></returns>
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
		/// <returns></returns>
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
		/// <returns></returns>
		public static object PostProcessRequestValue(object value, Type targetType)
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
#if IMPOSSIBLE_ODDS_VERBOSE
				Debug.LogWarningFormat("Target type {0} does not implement the {1} interface to post-process a value of type {2}.", targetType.Name, typeof(IConvertible).Name, value.GetType().Name);
#endif
				return value;
			}
		}
	}
}
