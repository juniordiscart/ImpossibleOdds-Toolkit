namespace ImpossibleOdds.DependencyInjection
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	public static class DependencyInjector
	{
		private readonly static HashSet<Type> rejectedTypesCache = new HashSet<Type>();
		private readonly static Dictionary<Type, TypeInjectionCache> typeInjectionCache = new Dictionary<Type, TypeInjectionCache>();
		private readonly static Dictionary<int, object[]> parametersCache = new Dictionary<int, object[]>();

		/// <summary>
		/// Clears the caches used by the dependency injector.
		/// </summary>
		public static void ClearCaches()
		{
			rejectedTypesCache.Clear();
			typeInjectionCache.Clear();
			parametersCache.Clear();
		}

		/// <summary>
		/// Creates an instance of the target type and injects the newly create object.
		/// Note: a suitable constructor is necessary to create the instance. Either a constructor marked
		/// with the 'Inject' attribute, or an accessable default constructor.
		/// </summary>
		/// <param name="container">Container with dependency bindings.</param>
		/// <typeparam name="T">The type of object to create an instance of.</typeparam>
		/// <returns>An instance of the target type, injected with resources found in the container.</returns>
		public static T CreateAndInject<T>(IDependencyContainer container)
		{
			return CreateAndInject<T>(container, string.Empty);
		}

		/// <summary>
		/// Creates an instance of the target type and injects the newly create object.
		/// Note: a suitable constructor is necessary to create the instance. Either a constructor marked
		/// with the 'Inject' attribute (with matching injection identifier), or an accessable default constructor.
		/// </summary>
		/// <param name="container">Container with dependency bindings.</param>
		/// <param name="injectionID">Only injects members with the injection ID.</param>
		/// <typeparam name="T">The type of object to create an instance of.</typeparam>
		/// <returns>An instance of the target type, injected with resources found in the container.</returns>
		public static T CreateAndInject<T>(IDependencyContainer container, string injectionId)
		{
			container.ThrowIfNull(nameof(container));

			Type targetType = typeof(T);
			if (targetType.IsInterface || targetType.IsAbstract)
			{
				throw new DependencyInjectionException("Cannot create an instance of type {0} because it is either an interface or declared abstract.", targetType.Name);
			}

			TypeInjectionCache typeInfo = GetTypeInjectionInfo(targetType);

			object instance = null;
			if (typeInfo.InjectableConstructors.Any(c => c.Attribute.IsInjectionIdDefined(string.Empty)))
			{
				MemberInjectionValue<ConstructorInfo> constructorInfo = typeInfo.InjectableConstructors.First(c => c.Attribute.IsInjectionIdDefined(string.Empty));
				ParameterInfo[] parameterInfo = constructorInfo.Member.GetParameters();
				object[] parameters = GetParameterInjectionList(parameterInfo.Length);
				FillPamaterInjectionList(parameterInfo, parameters, container);

				instance = (T)constructorInfo.Member.Invoke(parameters);
			}
			else
			{
				instance = Activator.CreateInstance(targetType, false);
			}

			// Inject the instance as well for fields, properties and other methods.
			Inject(container, instance);

			// Return the typed instance here so that any unboxing can be done.
			return (T)instance;
		}

		/// <summary>
		/// Inject the target object using the bindings found in the container.
		/// </summary>
		/// <param name="container">Container with dependency bindings.</param>
		/// <param name="target">Target to be injected.</param>
		public static void Inject(IDependencyContainer container, object target)
		{
			container.ThrowIfNull(nameof(container));
			target.ThrowIfNull(nameof(target));

			ResolveDependenciesForObject(target, container);
		}

		/// <summary>
		/// Inject the target object using the bindings found in the container.
		/// </summary>
		/// <param name="container">Container with dependency bindings.</param>
		/// <param name="injectionID">Only injects members with the injection ID.</param>
		/// <param name="target">Target to be injected.</param>
		public static void Inject(IDependencyContainer container, string injectionID, object target)
		{
			container.ThrowIfNull(nameof(container));
			injectionID.ThrowIfNullOrWhitespace(nameof(injectionID));
			target.ThrowIfNull(nameof(target));

			ResolveDependenciesForObject(target, container, injectionID);
		}

		/// <summary>
		/// Inject the target objects using the bindings found in the container.
		/// </summary>
		/// <param name="container">Container with dependency bindings.</param>
		/// <param name="targets">Targets to be injected.</param>
		public static void Inject(IDependencyContainer container, IEnumerable targets)
		{
			container.ThrowIfNull(nameof(container));
			targets.ThrowIfNull(nameof(targets));

			foreach (object target in targets)
			{
				if (target != null)
				{
					ResolveDependenciesForObject(target, container);
				}
			}
		}

		/// <summary>
		/// Inject the target objects using the bindings found in the container.
		/// </summary>
		/// <param name="container">Container with dependency bindings.</param>
		/// <param name="injectionID">Only injects members with the injection ID.</param>
		/// <param name="targets">Targets to be injected.</param>
		public static void Inject(IDependencyContainer container, string injectionID, IEnumerable targets)
		{
			container.ThrowIfNull(nameof(container));
			injectionID.ThrowIfNullOrWhitespace(nameof(injectionID));
			targets.ThrowIfNull(nameof(targets));

			foreach (object target in targets)
			{
				if (target != null)
				{
					ResolveDependenciesForObject(target, container, injectionID);
				}
			}
		}

		/// <summary>
		/// Inject the target objects using the bindings found in the container.
		/// </summary>
		/// <param name="container">Container with dependency bindings.</param>
		/// <param name="targets">Targets to be injected.</param>
		public static void Inject(IDependencyContainer container, params object[] targets)
		{
			if (targets.Length == 0)
			{
				return;
			}

			Inject(container, targets);
		}

		/// <summary>
		/// Inject the target objects using the bindings found in the container.
		/// </summary>
		/// <param name="container">Container with dependency bindings.</param>
		/// <param name="injectionID">Only injects members with the injection ID.</param>
		/// <param name="targets">Targets to be injected.</param>
		public static void Inject(IDependencyContainer container, string injectionID, params object[] targets)
		{
			if (targets.Length == 0)
			{
				return;
			}

			Inject(container, targets, injectionID);
		}

		private static void ResolveDependenciesForObject(object objToInject, IDependencyContainer container, string injectionID = null)
		{
			ResolveDependenciesForObject(objToInject, objToInject.GetType(), container, injectionID);
		}

		private static void ResolveDependenciesForObject(object objToInject, Type currentType, IDependencyContainer container, string injectionId = null)
		{
			// Don't inject on a null type, or an object type.
			if (currentType == null)
			{
				return;
			}

			// Inject the base types first, like we do with constructors.
			ResolveDependenciesForObject(objToInject, currentType.BaseType, container, injectionId);

			// Don't bother if the type is not injectable.
			if (!IsInjectable(currentType))
			{
				return;
			}

			TypeInjectionCache injectionInfo = GetTypeInjectionInfo(currentType);

			// Fields
			foreach (MemberInjectionValue<FieldInfo> field in injectionInfo.InjectableFields)
			{
				Type fieldType = field.Member.FieldType;
				if (field.Attribute.IsInjectionIdDefined(injectionId) && container.BindingExists(fieldType))
				{
					field.Member.SetValue(objToInject, container.GetBinding(fieldType).GetInstance());
				}
			}

			// Properties
			foreach (MemberInjectionValue<PropertyInfo> property in injectionInfo.InjectableProperties)
			{
				Type propertyType = property.Member.PropertyType;
				if (property.Attribute.IsInjectionIdDefined(injectionId) && container.BindingExists(propertyType))
				{
					property.Member.SetValue(objToInject, container.GetBinding(propertyType).GetInstance());
				}
			}

			// Methods
			foreach (MemberInjectionValue<MethodInfo> method in injectionInfo.InjectableMethods)
			{
				if (!method.Attribute.IsInjectionIdDefined(injectionId))
				{
					continue;
				}

				ParameterInfo[] parameterInfo = method.Member.GetParameters();
				object[] parameters = GetParameterInjectionList(parameterInfo.Length);
				FillPamaterInjectionList(parameterInfo, parameters, container);
				method.Member.Invoke(objToInject, parameters);
			}
		}

		private static object[] GetParameterInjectionList(int nrOfParams)
		{
			if (!parametersCache.ContainsKey(nrOfParams))
			{
				parametersCache.Add(nrOfParams, new object[nrOfParams]);
			}

			return parametersCache[nrOfParams];
		}

		private static void FillPamaterInjectionList(ParameterInfo[] parameterInfo, object[] parameters, IDependencyContainer container)
		{
			// Resolve the dependencies for the method parameters
			for (int i = 0; i < parameterInfo.Length; ++i)
			{
				ParameterInfo p = parameterInfo[i];
				Type parameterType = p.ParameterType;
				parameters[i] =
					container.BindingExists(parameterType) ?
					container.GetBinding(parameterType).GetInstance() :
					(parameterType.GetTypeInfo().IsValueType ? Activator.CreateInstance(parameterType, true) : null);
			}
		}

		private static TypeInjectionCache GetTypeInjectionInfo(Type type)
		{
			if (!typeInjectionCache.ContainsKey(type))
			{
				typeInjectionCache.Add(type, new TypeInjectionCache(type));
			}

			return typeInjectionCache[type];
		}

		private static bool IsInjectable(Type type)
		{
			if (rejectedTypesCache.Contains(type))
			{
				return false;
			}
			else if (typeInjectionCache.ContainsKey(type))
			{
				return true;
			}

			// Check for the injectable attribute
			if (!type.IsDefined(typeof(InjectableAttribute), false))
			{
				rejectedTypesCache.Add(type);
				return false;
			}
			else
			{
				return true;
			}
		}
	}
}
