namespace ImpossibleOdds
{
	using System;
	using System.Reflection;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using UnityEngine.Events;

	public static class DelegateExtensions
	{
		private static ConcurrentDictionary<Type, List<FieldInfo>> delegateCache = new ConcurrentDictionary<Type, List<FieldInfo>>();

		/// <summary>
		/// Clears the invokation lists of delegates found in the source that target the target.
		/// </summary>
		/// <param name="source">The object of which the delegates will be cleared.</param>
		/// <param name="target">The object to be removed from the delegate invocation lists.</param>
		public static void PurgeDelegatesOf(this object source, object target)
		{
			source.ThrowIfNull(nameof(source));
			target.ThrowIfNull(nameof(target));
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

		/// <summary>
		/// Invokes the action if it is not null.
		/// </summary>
		public static void InvokeIfNotNull(this Action del)
		{
			if (del != null)
			{
				del.Invoke();
			}
		}

		/// <summary>
		/// Invokes the action if it is not null.
		/// </summary>
		public static void InvokeIfNotNull<T1>(this Action<T1> del, T1 p1)
		{
			if (del != null)
			{
				del.Invoke(p1);
			}
		}

		/// <summary>
		/// Invokes the action if it is not null.
		/// </summary>
		public static void InvokeIfNotNull<T1, T2>(this Action<T1, T2> del, T1 p1, T2 p2)
		{
			if (del != null)
			{
				del.Invoke(p1, p2);
			}
		}

		/// <summary>
		/// Invokes the action if it is not null.
		/// </summary>
		public static void InvokeIfNotNull<T1, T2, T3>(this Action<T1, T2, T3> del, T1 p1, T2 p2, T3 p3)
		{
			if (del != null)
			{
				del.Invoke(p1, p2, p3);
			}
		}

		/// <summary>
		/// Invokes the action if it is not null.
		/// </summary>
		public static void InvokeIfNotNull<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> del, T1 p1, T2 p2, T3 p3, T4 p4)
		{
			if (del != null)
			{
				del.Invoke(p1, p2, p3, p4);
			}
		}

		/// <summary>
		/// Invokes the function if not null and returns the result. If the function is null, then the default value for type of TResult is returned.
		/// </summary>
		public static TResult InvokeIfNotNull<TResult>(this Func<TResult> func)
		{
			return (func != null) ? func() : default(TResult);
		}

		/// <summary>
		/// Invokes the function if not null and returns the result. If the function is null, then the default value for type of TResult is returned.
		/// </summary>
		public static TResult InvokeIfNotNull<T1, TResult>(this Func<T1, TResult> func, T1 p1)
		{
			return (func != null) ? func(p1) : default(TResult);
		}

		/// <summary>
		/// Invokes the function if not null and returns the result. If the function is null, then the default value for type of TResult is returned.
		/// </summary>
		public static TResult InvokeIfNotNull<T1, T2, TResult>(this Func<T1, T2, TResult> func, T1 p1, T2 p2)
		{
			return (func != null) ? func(p1, p2) : default(TResult);
		}

		/// <summary>
		/// Invokes the function if not null and returns the result. If the function is null, then the default value for type of TResult is returned.
		/// </summary>
		public static TResult InvokeIfNotNull<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func, T1 p1, T2 p2, T3 p3)
		{
			return (func != null) ? func(p1, p2, p3) : default(TResult);
		}

		/// <summary>
		/// Invokes the function if not null and returns the result. If the function is null, then the default value for type of TResult is returned.
		/// </summary>
		public static TResult InvokeIfNotNull<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func, T1 p1, T2 p2, T3 p3, T4 p4)
		{
			return (func != null) ? func(p1, p2, p3, p4) : default(TResult);
		}

		/// <summary>
		/// Invokes the action if it is not null.
		/// </summary>
		public static void InvokeIfNotNull(this UnityAction del)
		{
			if (del != null)
			{
				del.Invoke();
			}
		}

		/// <summary>
		/// Invokes the action if it is not null.
		/// </summary>
		public static void InvokeIfNotNull<T1>(this UnityAction<T1> del, T1 p1)
		{
			if (del != null)
			{
				del.Invoke(p1);
			}
		}

		/// <summary>
		/// Invokes the action if it is not null.
		/// </summary>
		public static void InvokeIfNotNull<T1, T2>(this UnityAction<T1, T2> del, T1 p1, T2 p2)
		{
			if (del != null)
			{
				del.Invoke(p1, p2);
			}
		}

		/// <summary>
		/// Invokes the action if it is not null.
		/// </summary>
		public static void InvokeIfNotNull<T1, T2, T3>(this UnityAction<T1, T2, T3> del, T1 p1, T2 p2, T3 p3)
		{
			if (del != null)
			{
				del.Invoke(p1, p2, p3);
			}
		}

		/// <summary>
		/// Invokes the action if it is not null.
		/// </summary>
		public static void InvokeIfNotNull<T1, T2, T3, T4>(this UnityAction<T1, T2, T3, T4> del, T1 p1, T2 p2, T3 p3, T4 p4)
		{
			if (del != null)
			{
				del.Invoke(p1, p2, p3, p4);
			}
		}

		/// <summary>
		/// Invokes the event if it is not null.
		/// </summary>
		public static void InvokeIfNotNull(this UnityEvent del)
		{
			if (del != null)
			{
				del.Invoke();
			}
		}

		/// <summary>
		/// Invokes the event if it is not null.
		/// </summary>
		public static void InvokeIfNotNull<T1>(this UnityEvent<T1> del, T1 p1)
		{
			if (del != null)
			{
				del.Invoke(p1);
			}
		}

		/// <summary>
		/// Invokes the event if it is not null.
		/// </summary>
		public static void InvokeIfNotNull<T1, T2>(this UnityEvent<T1, T2> del, T1 p1, T2 p2)
		{
			if (del != null)
			{
				del.Invoke(p1, p2);
			}
		}

		/// <summary>
		/// Invokes the event if it is not null.
		/// </summary>
		public static void InvokeIfNotNull<T1, T2, T3>(this UnityEvent<T1, T2, T3> del, T1 p1, T2 p2, T3 p3)
		{
			if (del != null)
			{
				del.Invoke(p1, p2, p3);
			}
		}

		/// <summary>
		/// Invokes the event if it is not null.
		/// </summary>
		public static void InvokeIfNotNull<T1, T2, T3, T4>(this UnityEvent<T1, T2, T3, T4> del, T1 p1, T2 p2, T3 p3, T4 p4)
		{
			if (del != null)
			{
				del.Invoke(p1, p2, p3, p4);
			}
		}

		private static List<FieldInfo> GetDelegatesForType(Type type)
		{
			if (delegateCache.TryGetValue(type, out List<FieldInfo> d))
			{
				return d;
			}

			Type delegateType = typeof(Delegate);
			BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

			// Find all delegate fields through the type hierarchy.
			List<FieldInfo> delegates = new List<FieldInfo>();
			Type typeIt = type;
			while ((typeIt != null) && (typeIt != typeof(object)))
			{
				FieldInfo[] fields = typeIt.GetFields(flags);

				foreach (FieldInfo field in fields)
				{
					if ((field.DeclaringType == typeIt) && delegateType.IsAssignableFrom(field.FieldType))
					{
						delegates.Add(field);
					}
				}

				typeIt = typeIt.BaseType;
			}

			return delegateCache.GetOrAdd(type, delegates);
		}
	}
}
