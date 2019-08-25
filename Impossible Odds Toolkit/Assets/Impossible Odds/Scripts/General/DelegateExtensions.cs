namespace ImpossibleOdds
{
	using System;
	using System.Reflection;
	using System.Collections.Generic;

	public static class DelegateExtensions
	{
		private static Dictionary<Type, List<FieldInfo>> delegateCache = new Dictionary<Type, List<FieldInfo>>();

		/// <summary>
		/// Clears the invokation lists of delegates found in the source that target the target.
		/// </summary>
		/// <param name="source">The object of which the delegates will be cleared.</param>
		/// <param name="target">The object to be removed from the invocation lists.</param>
		public static void PurgeDelegatesOf(this object source, object target)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			else if (target == null)
			{
				throw new ArgumentNullException("target");
			}

			Type sourceType = source is Type ? source as Type : source.GetType();
			bool isStaticSource = source is Type; // Source - instance or static?
			bool isStaticTarget = target is Type; // Target - instance or static?

			List<FieldInfo> fields = GetDelegatesForType(sourceType);
			foreach (FieldInfo delegateInfo in fields)
			{
				// Mismatch in static/instance source object - field declaration
				if (isStaticSource != delegateInfo.IsStatic)
				{
					continue;
				}

				Delegate eventDelegate = delegateInfo.GetValue(isStaticSource ? null : source) as Delegate;
				if (eventDelegate == null)
				{
					continue;
				}

				Delegate[] delegates = eventDelegate.GetInvocationList();
				foreach (Delegate del in delegates)
				{
					// Mismatch in static/instance target object - field declaration
					if ((del.Method == null) || (isStaticTarget != del.Method.IsStatic))
					{
						continue;
					}

					// Remove the handler if the target matches or, in case of static target, the type matches
					// the declaring type of the method associated with the delegate method.
					if ((!isStaticTarget && (del.Target == target)) || (del.Method.DeclaringType == target as Type))
					{
						eventDelegate = Delegate.Remove(eventDelegate, del);
					}
				}

				delegateInfo.SetValue(isStaticSource ? null : source, eventDelegate);
			}
		}

		private static List<FieldInfo> GetDelegatesForType(Type type)
		{
			if (delegateCache.ContainsKey(type))
			{
				return delegateCache[type];
			}

			Type delegateType = typeof(Delegate);
			BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

			List<FieldInfo> delegates = new List<FieldInfo>();
			delegateCache.Add(type, delegates);

			// Find all delegate fields through the type hierarchy.
			while ((type != null) && (type != typeof(object)))
			{
				FieldInfo[] fields = type.GetFields(flags);

				foreach (FieldInfo field in fields)
				{
					if ((field.DeclaringType == type) && delegateType.IsAssignableFrom(field.FieldType))
					{
						delegates.Add(field);
					}
				}

				type = type.BaseType;
			}

			return delegates;
		}
	}
}
