namespace ImpossibleOdds.DependencyInjection
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Concurrent;
	using System.Linq;
	using System.Reflection;
	using ImpossibleOdds.ReflectionCaching;

	public static class DependencyInjector
	{
		private readonly static HashSet<Type> rejectedTypes = null;
		private readonly static ConcurrentDictionary<Type, TypeInjectionCache> typeInjectionCache = null;

		static DependencyInjector()
		{
			int count = 0;
			rejectedTypes = new HashSet<Type>();
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type type in assembly.GetTypes())
				{
					if (!Attribute.IsDefined(type, typeof(InjectableAttribute), true))
					{
						lock (rejectedTypes)
						{
							rejectedTypes.Add(type);
						}
					}
					else
					{
						count++;
					}
				}
			}

			// Set a concurrency level of 4. No real reason why this was chosen.
			typeInjectionCache = new ConcurrentDictionary<Type, TypeInjectionCache>(4, count);
		}

		/// <summary>
		/// Clears the caches used by the dependency injector.
		/// </summary>
		public static void ClearCaches()
		{
			typeInjectionCache.Clear();

			// Don't clear this one, as it's statically filled once, and will not work properly anymore otherwise.
			// This is a performance optimization.
			// rejectedTypes.Clear();
		}

		/// <summary>
		/// Creates an instance of the target type and injects the newly create object.
		/// Note: a suitable constructor is necessary to create the instance. Either a constructor marked
		/// with the 'Inject' attribute, or an accessible default constructor.
		/// </summary>
		/// <param name="container">Container with resources to inject.</param>
		/// <typeparam name="T">The type of object to create an instance of.</typeparam>
		/// <returns>An instance of the target type, injected with resources found in the container.</returns>
		public static T CreateAndInject<T>(IReadOnlyDependencyContainer container)
		{
			return CreateAndInject<T>(container, string.Empty);
		}

		/// <summary>
		/// Creates an instance of the target type and injects the newly create object.
		/// Note: a suitable constructor is necessary to create the instance. Either a constructor marked
		/// with the 'Inject' attribute, or an accessible default constructor.
		/// </summary>
		/// <param name="targetType">The type of object to create an instance of.</param>
		/// <param name="container">Container with resources to inject.</param>
		/// <returns>An instance of the target type, injected with resources found in the container.</returns>
		public static object CreateAndInject(Type targetType, IReadOnlyDependencyContainer container)
		{
			return CreateAndInject(targetType, container, string.Empty);
		}

		/// <summary>
		/// Creates an instance of the target type and injects the newly create object.
		/// Note: a suitable constructor is necessary to create the instance. Either a constructor marked
		/// with the 'Inject' attribute (with matching injection identifier), or an accessible default constructor.
		/// </summary>
		/// <param name="container">Container with resources to inject.</param>
		/// <param name="injectionId">Only injects members with the same injection identifier.</param>
		/// <typeparam name="T">The type of object to create an instance of.</typeparam>
		/// <returns>An instance of the target type, injected with resources found in the container.</returns>
		public static T CreateAndInject<T>(IReadOnlyDependencyContainer container, string injectionId)
		{
			return (T)CreateAndInject(typeof(T), container, injectionId);
		}

		/// <summary>
		/// Creates an instance of the target type and injects the newly create object.
		/// Note: a suitable constructor is necessary to create the instance. Either a constructor marked
		/// with the 'Inject' attribute (with matching injection identifier), or an accessible default constructor.
		/// </summary>
		/// <param name="targetType">The type of object to create an instance of.</param>
		/// <param name="container">Container with resources to inject.</param>
		/// <param name="injectionId">Only injects members with the same injection identifier.</param>
		/// <returns>An instance of the target type, injected with resources found in the container.</returns>
		public static object CreateAndInject(Type targetType, IReadOnlyDependencyContainer container, string injectionId)
		{
			targetType.ThrowIfNull(nameof(targetType));
			container.ThrowIfNull(nameof(container));

			if (targetType.IsInterface || targetType.IsAbstract)
			{
				throw new DependencyInjectionException("Cannot create an instance of type {0} because it is either an interface or declared abstract.", targetType.Name);
			}

			object instance = null;
			if (TryGetTypeInjectionInfo(targetType, out TypeInjectionCache typeInfo) && typeInfo.InjectableConstructors.Any(c => c.Attribute.IsInjectionIdDefined(string.Empty)))
			{
				MemberInjectionValue<ConstructorInfo> constructorInfo = typeInfo.InjectableConstructors.First(c => c.Attribute.IsInjectionIdDefined(string.Empty));
				ParameterInfo[] parameterInfo = constructorInfo.Member.GetParameters();
				object[] parameters = TypeReflectionUtilities.GetParameterInvokationList(parameterInfo.Length);
				FillParameterInjectionList(parameterInfo, parameters, container);
				instance = constructorInfo.Member.Invoke(parameters);
				TypeReflectionUtilities.ReturnParameterInvokationList(parameters);
			}
			else
			{
				instance = Activator.CreateInstance(targetType, false);
			}

			// Inject the instance as well for fields, properties and other methods.
			Inject(container, instance);
			return instance;
		}

		/// <summary>
		/// Injects the static members of a type.
		/// </summary>
		/// <param name="container">Container with resources to inject.</param>
		/// <param name="type">The type for which to inject its static members.</param>
		public static void InjectStatic(IReadOnlyDependencyContainer container, Type type)
		{
			container.ThrowIfNull(nameof(container));
			type.ThrowIfNull(nameof(type));

			ResolveDependenciesForObject(type, container);
		}

		/// <summary>
		/// Injects the static members of a type.
		/// </summary>
		/// <param name="container">Container with resources to inject.</param>
		/// <param name="type">The type for which to inject its static members.</param>
		/// <param name="injectionId">Only injects members with this injection identifier defined.</param>
		public static void InjectStatic(IReadOnlyDependencyContainer container, string injectionId, Type type)
		{
			container.ThrowIfNull(nameof(container));
			type.ThrowIfNull(nameof(type));

			ResolveDependenciesForObject(type, container, injectionId);
		}

		/// <summary>
		/// Inject all static members using the resources found in the provided container.
		/// </summary>
		/// <param name="container">Container with resources to inject.</param>
		public static void InjectStaticAll(IReadOnlyDependencyContainer container)
		{
			container.ThrowIfNull(nameof(container));

			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type type in assembly.GetTypes())
				{
					if (!rejectedTypes.Contains(type))
					{
						ResolveDependenciesForObject(type, container);
					}
				}
			}
		}

		/// <summary>
		/// Inject all static members using the resources found in the provided container.
		/// </summary>
		/// <param name="container">Container with resources to inject.</param>
		/// <param name="injectionId">Only injects members with this injection identifier defined.</param>
		public static void InjectStaticAll(IReadOnlyDependencyContainer container, string injectionId)
		{
			container.ThrowIfNull(nameof(container));

			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type type in assembly.GetTypes())
				{
					if (!rejectedTypes.Contains(type))
					{
						ResolveDependenciesForObject(type, container, injectionId);
					}
				}
			}
		}

		/// <summary>
		/// Inject the target object using the resources found in the provided container.
		/// Note: if the target is of type Type, then it's static members will be injected.
		/// </summary>
		/// <param name="container">Container with dependency resources.</param>
		/// <param name="target">Target to be injected.</param>
		public static void Inject(IReadOnlyDependencyContainer container, object target)
		{
			container.ThrowIfNull(nameof(container));
			target.ThrowIfNull(nameof(target));

			ResolveDependenciesForObject(target, container);
		}

		/// <summary>
		/// Inject the target object using the resources found in the provided container.
		/// Note: if the target is of type Type, then it's static members will be injected.
		/// </summary>
		/// <param name="container">Container with dependency resources.</param>
		/// <param name="injectionId">Only injects members with the injection identifier.</param>
		/// <param name="target">Target to be injected.</param>
		public static void Inject(IReadOnlyDependencyContainer container, string injectionId, object target)
		{
			container.ThrowIfNull(nameof(container));
			injectionId.ThrowIfNullOrWhitespace(nameof(injectionId));
			target.ThrowIfNull(nameof(target));

			ResolveDependenciesForObject(target, container, injectionId);
		}

		/// <summary>
		/// Inject the target objects using the resources found in the provided container.
		/// </summary>
		/// <param name="container">Container with dependency resources.</param>
		/// <param name="targets">Targets to be injected.</param>
		public static void Inject(IReadOnlyDependencyContainer container, IEnumerable targets)
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
		/// Inject the target objects using the resources found in the provided container.
		/// </summary>
		/// <param name="container">Container with dependency resources.</param>
		/// <param name="injectionId">Only injects members with the injection ID.</param>
		/// <param name="targets">Targets to be injected.</param>
		public static void Inject(IReadOnlyDependencyContainer container, string injectionId, IEnumerable targets)
		{
			container.ThrowIfNull(nameof(container));
			injectionId.ThrowIfNullOrWhitespace(nameof(injectionId));
			targets.ThrowIfNull(nameof(targets));

			foreach (object target in targets)
			{
				if (target != null)
				{
					ResolveDependenciesForObject(target, container, injectionId);
				}
			}
		}

		/// <summary>
		/// Inject the target objects using the resources found in the provided container.
		/// </summary>
		/// <param name="container">Container with dependency resources.</param>
		/// <param name="targets">Targets to be injected.</param>
		public static void Inject(IReadOnlyDependencyContainer container, params object[] targets)
		{
			if (targets.Length == 0)
			{
				return;
			}

			Inject(container, targets);
		}

		/// <summary>
		/// Inject the target objects using the resources found in the provided container.
		/// </summary>
		/// <param name="container">Container with dependency resources.</param>
		/// <param name="injectionId">Only injects members with the injection ID.</param>
		/// <param name="targets">Targets to be injected.</param>
		public static void Inject(IReadOnlyDependencyContainer container, string injectionId, params object[] targets)
		{
			if (targets.Length == 0)
			{
				return;
			}

			Inject(container, targets, injectionId);
		}

		/// <summary>
		/// Resolve the dependencies of the object using the resources found in the given container.
		/// </summary>
		/// <param name="objToInject">The object to inject with resources.</param>
		/// <param name="container">The resources to inject into the object.</param>
		/// <param name="injectionId">An identifier to restrict to specifically defined scopes.</param>
		private static void ResolveDependenciesForObject(object objToInject, IReadOnlyDependencyContainer container, string injectionId = null)
		{
			Type type = (objToInject is Type) ? (objToInject as Type) : objToInject.GetType();
			ResolveFieldDependencies(objToInject, type, container, injectionId);
			ResolvePropertyDependencies(objToInject, type, container, injectionId);
			ResolveMethodDependencies(objToInject, type, container, injectionId);
		}

		/// <summary>
		/// Resolve all injectable fields across the type-chain.
		/// </summary>
		/// <param name="objToInject">The object for which to inject its fields.</param>
		/// <param name="currentType">The current type across the type chain.</param>
		/// <param name="container">The resource container.</param>
		/// <param name="injectionId">An identifier to restrict to specifically defined scopes.</param>
		private static void ResolveFieldDependencies(object objToInject, Type currentType, IReadOnlyDependencyContainer container, string injectionId = null)
		{
			if (TryGetTypeInjectionInfo(currentType, out TypeInjectionCache cache))
			{
				bool isStaticInjection = objToInject is Type;

				foreach (MemberInjectionValue<FieldInfo> field in cache.InjectableFields)
				{
					Type fieldType = field.Member.FieldType;
					if (field.Attribute.IsInjectionIdDefined(injectionId) && container.BindingExists(fieldType))
					{
						if (isStaticInjection && field.Member.IsStatic)
						{
							field.Member.SetValue(null, container.GetBinding(fieldType).GetInstance());
						}
						else if (!isStaticInjection && !field.Member.IsStatic)
						{
							field.Member.SetValue(objToInject, container.GetBinding(fieldType).GetInstance());
						}
					}
				}
			}
		}

		/// <summary>
		/// Resolve all injectable properties across the type-chain.
		/// </summary>
		/// <param name="objToInject">The object for which to inject its properties.</param>
		/// <param name="currentType">The current type across the type chain.</param>
		/// <param name="container">The resource container.</param>
		/// <param name="injectionId">An identifier to restrict to specifically defined scopes.</param>
		private static void ResolvePropertyDependencies(object objToInject, Type currentType, IReadOnlyDependencyContainer container, string injectionId = null)
		{
			if (TryGetTypeInjectionInfo(currentType, out TypeInjectionCache cache))
			{
				bool isStaticInjection = objToInject is Type;

				foreach (MemberInjectionValue<PropertyInfo> property in cache.InjectableProperties)
				{
					Type propertyType = property.Member.PropertyType;
					if (property.Attribute.IsInjectionIdDefined(injectionId) && container.BindingExists(propertyType))
					{
						if (isStaticInjection && property.Member.SetMethod.IsStatic)
						{
							property.Member.SetValue(null, container.GetBinding(propertyType).GetInstance());
						}
						else if (!isStaticInjection && !property.Member.SetMethod.IsStatic)
						{
							property.Member.SetValue(objToInject, container.GetBinding(propertyType).GetInstance());
						}
					}
				}
			}
		}

		/// <summary>
		/// Resolve all injectable methods across the type-chain.
		/// </summary>
		/// <param name="objToInject">The object for which to inject its methods.</param>
		/// <param name="currentType">The current type across the type chain.</param>
		/// <param name="container">The resource container.</param>
		/// <param name="injectionId">An identifier to restrict to specifically defined scopes.</param>
		private static void ResolveMethodDependencies(object objToInject, Type currentType, IReadOnlyDependencyContainer container, string injectionId = null)
		{
			if (TryGetTypeInjectionInfo(currentType, out TypeInjectionCache cache))
			{
				bool isStaticInjection = objToInject is Type;

				foreach (MemberInjectionValue<MethodInfo> method in cache.InjectableMethods)
				{
					if (method.Attribute.IsInjectionIdDefined(injectionId))
					{
						if (isStaticInjection && method.Member.IsStatic)
						{
							ParameterInfo[] parameterInfo = method.Member.GetParameters();
							object[] parameters = TypeReflectionUtilities.GetParameterInvokationList(parameterInfo.Length);
							FillParameterInjectionList(parameterInfo, parameters, container);
							method.Member.Invoke(null, parameters);
							TypeReflectionUtilities.ReturnParameterInvokationList(parameters);
						}
						else if (!isStaticInjection && !method.Member.IsStatic)
						{
							ParameterInfo[] parameterInfo = method.Member.GetParameters();
							object[] parameters = TypeReflectionUtilities.GetParameterInvokationList(parameterInfo.Length);
							FillParameterInjectionList(parameterInfo, parameters, container);
							method.Member.Invoke(objToInject, parameters);
							TypeReflectionUtilities.ReturnParameterInvokationList(parameters);
						}
					}
				}
			}
		}

		private static void FillParameterInjectionList(ParameterInfo[] parameterInfo, object[] parameters, IReadOnlyDependencyContainer container)
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

		private static bool TryGetTypeInjectionInfo(Type type, out TypeInjectionCache typeInjection)
		{
			if (rejectedTypes.Contains(type))
			{
				typeInjection = null;
				return false;
			}
			else
			{
				typeInjection = typeInjectionCache.GetOrAdd(type, (t) => new TypeInjectionCache(t));
				return true;
			}
		}
	}
}
