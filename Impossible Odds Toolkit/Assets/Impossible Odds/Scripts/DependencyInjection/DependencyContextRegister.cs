namespace ImpossibleOdds.DependencyInjection
{
	using ImpossibleOdds;
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// A registry for dependency injection contexts.
	/// </summary>
	public static class DependencyContextRegister
	{
		private static Dictionary<object, IDependencyContext> contextRegister = new Dictionary<object, IDependencyContext>();

		/// <summary>
		/// Register a context with a given key. If the key is already taken, the value is overridden.
		/// </summary>
		/// <param name="key">The key to which to register the context.</param>
		/// <param name="context">The context to store.</param>
		public static void Register(object key, IDependencyContext context)
		{
			key.ThrowIfNull(nameof(key));
			context.ThrowIfNull(nameof(context));

			if (Exists(key))
			{
#if IMPOSSIBLE_ODDS_VERBOSE
				Debug.LogWarningFormat("A context registered to key {0} already exists. Overriding with new context.", key.ToString());
#endif
				contextRegister[key] = context;
			}
			else
			{
				contextRegister.Add(key, context);
			}
		}

		/// <summary>
		/// Remove the context registration that is registered to the given key
		/// </summary>
		/// <param name="key">Key to remove from the register.</param>
		public static void Remove(object key)
		{
			key.ThrowIfNull(nameof(key));
			if (Exists(key))
			{
				contextRegister.Remove(key);
			}
		}

		/// <summary>
		/// Checks whether the a context is registered to the given key.
		/// </summary>
		/// <param name="key">The key to check for.</param>
		/// <returns>True if a non-null value is registered.</returns>
		public static bool Exists(object key)
		{
			key.ThrowIfNull(nameof(key));
			return contextRegister.ContainsKey(key) && (contextRegister[key] != null);
		}

		/// <summary>
		/// Get the context registered to a specific key.
		/// </summary>
		/// <param name="key">The key to retrieve the context.</param>
		/// <returns>Returns the context registered to the key. Null otherwise.</returns>
		public static IDependencyContext GetContext(object key)
		{
			key.ThrowIfNull(nameof(key));

			if (Exists(key))
			{
				return contextRegister[key];
			}
			else
			{
#if IMPOSSIBLE_ODDS_VERBOSE
				Debug.LogWarningFormat("The dependency context register does not have a context registered under key {0}", key.ToString());
#endif
				return null;
			}
		}
	}
}
