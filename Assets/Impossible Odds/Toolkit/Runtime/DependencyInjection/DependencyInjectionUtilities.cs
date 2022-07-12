namespace ImpossibleOdds.DependencyInjection
{
	using System;
	using System.Collections.Generic;

	public static class DependencyInjectionUtilities
	{
		/// <summary>
		/// Get the current type and implemented interfaces for the given object.
		/// </summary>
		/// <param name="obj">Object to retrieve the type and interfaces for.</param>
		/// <returns>A collection of the current type along with implemented interfaces.</returns>
		public static IEnumerable<Type> GetTypeAndInterfaces(object obj)
		{
			obj.ThrowIfNull(nameof(obj));
			return GetTypeAndInterfaces(obj.GetType());
		}

		/// <summary>
		/// Get the implemented interfaces by the given type as well as the type iteself again.
		/// </summary>
		/// <typeparam name="T">Type to retieve the interfaces for.</typeparam>
		/// <returns>An array with the implemented interfaces and the type itself again (at index 0).</returns>
		public static IEnumerable<Type> GetTypeAndInterfaces<T>()
		{
			return GetTypeAndInterfaces(typeof(T));
		}

		/// <summary>
		/// Get the implemented interfaces by the given type as well as the type itself again.
		/// </summary>
		/// <param name="type">Type to retieve the interfaces for.</param>
		/// <returns>An array with the implemented interfaces and the type itself again (at index 0).</returns>
		public static IEnumerable<Type> GetTypeAndInterfaces(Type type)
		{
			type.ThrowIfNull(nameof(type));

			Type[] interfaces = type.GetInterfaces();
			List<Type> t = new List<Type>(interfaces.Length + 1);
			t.Add(type);
			t.AddRange(interfaces);

			return t;
		}
	}
}
