namespace ImpossibleOdds.DependencyInjection
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	public static class DependencyInjector
	{
		private static Dictionary<Type, TypeInjectionInfo> typeInjectionCache = new Dictionary<Type, TypeInjectionInfo>();

		static DependencyInjector()
		{ }

		public static void Inject(IDependencyContainer container, object target)
		{
			container.ThrowIfNull(nameof(container));
			target.ThrowIfNull(nameof(target));
			ResolveDependencies(target, container);
		}

		public static void Inject(IDependencyContainer container, IEnumerable targets)
		{
			container.ThrowIfNull(nameof(container));
			targets.ThrowIfNull(nameof(targets));
			foreach (object target in targets)
			{
				if (target != null)
				{
					ResolveDependencies(target, container);
				}
			}
		}

		public static void Inject(IDependencyContainer container, params object[] targets)
		{
			if (targets.Length == 0)
			{
				return;
			}

			Inject(container, targets);
		}

		private static void ResolveDependencies(object objToInject, IDependencyContainer container)
		{
			container.ThrowIfNull(nameof(container));
			Type objType = typeof(object);
			Type currentType = objToInject.GetType();
			while ((currentType != objType) && (currentType != null))
			{
				TypeInjectionInfo injectionInfo = GetTypeInjectionInfo(currentType);

				// Fields
				foreach (FieldInfo field in injectionInfo.injectableFields)
				{
					Type fieldType = field.FieldType;
					if (container.BindingExists(fieldType))
					{
						field.SetValue(objToInject, container.GetBinding(fieldType).GetInstance());
					}
				}

				// Properties
				foreach (PropertyInfo property in injectionInfo.injectableProperties)
				{
					Type propertyType = property.PropertyType;
					if (container.BindingExists(propertyType))
					{
						property.SetValue(objToInject, container.GetBinding(propertyType).GetInstance());
					}
				}

				// Methods
				foreach (MethodInfo method in injectionInfo.injectableMethods)
				{
					ParameterInfo[] parameterInfo = method.GetParameters();
					object[] parameters = new object[parameterInfo.Length];

					// Resolve the dependencies for the method parameters
					for (int i = 0; i < parameterInfo.Length; ++i)
					{
						ParameterInfo p = parameterInfo[i];
						Type parameterType = p.ParameterType;
						if (container.BindingExists(parameterType))
						{
							parameters[i] = container.GetBinding(parameterType).GetInstance();
						}
						else
						{
							parameters[i] = parameterType.IsValueType ? Activator.CreateInstance(parameterType) : null;
						}
					}

					method.Invoke(objToInject, parameters);
				}

				currentType = currentType.BaseType;
			}
		}

		private static TypeInjectionInfo GetTypeInjectionInfo(Type type)
		{
			if (!typeInjectionCache.ContainsKey(type))
			{
				TypeInjectionInfo injectionCache = ConstructTypeInjectables(type);
				typeInjectionCache.Add(type, injectionCache);
			}

			return typeInjectionCache[type];
		}

		/// <summary>
		/// Retrieve all members, properties and methods that have been marked as injectable.
		/// </summary>
		/// <param name="type">Type of the object to request the cache for.</param>
		/// <returns></returns>
		private static TypeInjectionInfo ConstructTypeInjectables(Type type)
		{
			IEnumerable<FieldInfo> injectableFields = type.
				GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).
				Where(f => f.IsDefined(typeof(InjectAttribute), false) && !f.IsLiteral);    // Exclude constants.

			IEnumerable<PropertyInfo> injectableProperties = type.
				GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).
				Where(f => f.IsDefined(typeof(InjectAttribute), false) && f.CanWrite);  // Exclude properties we can't write to.

			IEnumerable<MethodInfo> injectableMethods = type.
				GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).
				Where(f => f.IsDefined(typeof(InjectAttribute), false));

			TypeInjectionInfo typeCache = new TypeInjectionInfo(type, injectableFields, injectableProperties, injectableMethods);
			return typeCache;
		}

		private class TypeInjectionInfo
		{
			public readonly Type type;
			public readonly IEnumerable<FieldInfo> injectableFields;
			public readonly IEnumerable<PropertyInfo> injectableProperties;
			public readonly IEnumerable<MethodInfo> injectableMethods;

			public TypeInjectionInfo(Type type, IEnumerable<FieldInfo> injectableFields, IEnumerable<PropertyInfo> injectableProperties, IEnumerable<MethodInfo> injectableMethods)
			{
				this.type = type;
				this.injectableFields = injectableFields;
				this.injectableProperties = injectableProperties;
				this.injectableMethods = injectableMethods;
			}
		}
	}
}
