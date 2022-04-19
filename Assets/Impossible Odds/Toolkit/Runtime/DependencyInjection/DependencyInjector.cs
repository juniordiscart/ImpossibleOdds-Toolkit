namespace ImpossibleOdds.DependencyInjection
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	public static class DependencyInjector
	{
		private readonly static Type ObjectType = typeof(object);
		private readonly static Dictionary<Type, TypeInjectionInfo> typeInjectionCache = new Dictionary<Type, TypeInjectionInfo>();
		private readonly static HashSet<Type> rejectedTypes = new HashSet<Type>();

		// Cache for parameter lists of injectable methods
		private readonly static Dictionary<int, object[]> parameterCacheList = new Dictionary<int, object[]>();

		/// <summary>
		/// Creates an instance of the target type and injects the newly create object.
		/// Note: a suitable constructor is necessary to create the instance. Either a constructor marked
		/// with the 'Inject' attribute, or an accessable default constructor.
		/// </summary>
		/// <param name="container">Container with resources to inject.</param>
		/// <typeparam name="T">The type of object to create an instance of.</typeparam>
		/// <returns>An instance of the target type, injected with resources found in the container.</returns>
		public static T CreateAndInject<T>(IDependencyContainer container)
		{
			return CreateAndInject<T>(container, string.Empty);
		}

		/// <summary>
		/// Creates an instance of the target type and injects the newly create object.
		/// Note: a suitable constructor is necessary to create the instance. Either a constructor marked
		/// with the 'Inject' attribute, or an accessable default constructor.
		/// </summary>
		/// <param name="targetType">The type of object to create an instance of.</param>
		/// <param name="container">Container with resources to inject.</param>
		/// <returns>An instance of the target type, injected with resources found in the container.</returns>
		public static object CreateAndInject(Type targetType, IDependencyContainer container)
		{
			return CreateAndInject(targetType, container, string.Empty);
		}

		/// <summary>
		/// Creates an instance of the target type and injects the newly create object.
		/// Note: a suitable constructor is necessary to create the instance. Either a constructor marked
		/// with the 'Inject' attribute (with matching injection identifier), or an accessable default constructor.
		/// </summary>
		/// <param name="container">Container with resources to inject.</param>
		/// <param name="injectionId">Only injects members with the same injection identifier.</param>
		/// <typeparam name="T">The type of object to create an instance of.</typeparam>
		/// <returns>An instance of the target type, injected with resources found in the container.</returns>
		public static T CreateAndInject<T>(IDependencyContainer container, string injectionId)
		{
			return (T)CreateAndInject(typeof(T), container, injectionId);
		}

		/// <summary>
		/// Creates an instance of the target type and injects the newly create object.
		/// Note: a suitable constructor is necessary to create the instance. Either a constructor marked
		/// with the 'Inject' attribute (with matching injection identifier), or an accessable default constructor.
		/// </summary>
		/// <param name="targetType">The type of object to create an instance of.</param>
		/// <param name="container">Container with resources to inject.</param>
		/// <param name="injectionId">Only injects members with the same injection identifier.</param>
		/// <returns>An instance of the target type, injected with resources found in the container.</returns>
		public static object CreateAndInject(Type targetType, IDependencyContainer container, string injectionId)
		{
			targetType.ThrowIfNull(nameof(targetType));
			container.ThrowIfNull(nameof(container));

			if (targetType.IsInterface || targetType.IsAbstract)
			{
				throw new DependencyInjectionException("Cannot create an instance of type {0} because it is either an interface or declared abstract.", targetType.Name);
			}

			TypeInjectionInfo typeInfo = GetTypeInjectionInfo(targetType);

			object instance = null;
			if (typeInfo.injectableConstructors.Any(c => c.IsInjectionScopeDefined(string.Empty)))
			{
				ConstructorInfo constructor = typeInfo.injectableConstructors.First(c => c.IsInjectionScopeDefined(string.Empty)).member;
				ParameterInfo[] parameterInfo = constructor.GetParameters();
				object[] parameters = GetParameterInjectionList(parameterInfo.Length);
				FillPamaterInjectionList(parameterInfo, parameters, container);

				instance = constructor.Invoke(parameters);
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
		/// <param name="injectionId">Only injects members with the injection ID.</param>
		/// <param name="target">Target to be injected.</param>
		public static void Inject(IDependencyContainer container, string injectionId, object target)
		{
			container.ThrowIfNull(nameof(container));
			injectionId.ThrowIfNullOrWhitespace(nameof(injectionId));
			target.ThrowIfNull(nameof(target));

			ResolveDependenciesForObject(target, container, injectionId);
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
		/// <param name="injectionId">Only injects members with the injection ID.</param>
		/// <param name="targets">Targets to be injected.</param>
		public static void Inject(IDependencyContainer container, string injectionId, IEnumerable targets)
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

		private static void ResolveDependenciesForObject(object objToInject, Type currentType, IDependencyContainer container, string injectionID = null)
		{
			// Don't inject on a null type, or an object type.
			if ((currentType == null) || (currentType == ObjectType))
			{
				return;
			}

			// Inject the base types first, like we do with constructors.
			ResolveDependenciesForObject(objToInject, currentType.BaseType, container, injectionID);

			// Don't bother if the type is not injectable.
			if (!IsInjectable(currentType))
			{
				return;
			}

			TypeInjectionInfo injectionInfo = GetTypeInjectionInfo(currentType);

			// Fields
			foreach (InjectableMemberInfo<FieldInfo> field in injectionInfo.injectableFields)
			{
				Type fieldType = field.member.FieldType;
				if (field.IsInjectionScopeDefined(injectionID) && container.BindingExists(fieldType))
				{
					field.member.SetValue(objToInject, container.GetBinding(fieldType).GetInstance());
				}
			}

			// Properties
			foreach (InjectableMemberInfo<PropertyInfo> property in injectionInfo.injectableProperties)
			{
				Type propertyType = property.member.PropertyType;
				if (property.IsInjectionScopeDefined(injectionID) && container.BindingExists(propertyType))
				{
					property.member.SetValue(objToInject, container.GetBinding(propertyType).GetInstance());
				}
			}

			// Methods
			foreach (InjectableMemberInfo<MethodInfo> method in injectionInfo.injectableMethods)
			{
				if (!method.IsInjectionScopeDefined(injectionID))
				{
					continue;
				}

				ParameterInfo[] parameterInfo = method.member.GetParameters();
				object[] parameters = GetParameterInjectionList(parameterInfo.Length);
				FillPamaterInjectionList(parameterInfo, parameters, container);
				method.member.Invoke(objToInject, parameters);
			}
		}

		private static object[] GetParameterInjectionList(int nrOfParams)
		{
			if (!parameterCacheList.ContainsKey(nrOfParams))
			{
				parameterCacheList.Add(nrOfParams, new object[nrOfParams]);
			}

			return parameterCacheList[nrOfParams];
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

		private static TypeInjectionInfo GetTypeInjectionInfo(Type type)
		{
			if (!typeInjectionCache.ContainsKey(type))
			{
				typeInjectionCache.Add(type, new TypeInjectionInfo(type));
			}

			return typeInjectionCache[type];
		}

		private static bool IsInjectable(Type type)
		{
			if (rejectedTypes.Contains(type))
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
				rejectedTypes.Add(type);
				return false;
			}
			else
			{
				return true;
			}
		}

		private class TypeInjectionInfo
		{
			public readonly Type type;
			public readonly IEnumerable<InjectableMemberInfo<ConstructorInfo>> injectableConstructors;
			public readonly IEnumerable<InjectableMemberInfo<FieldInfo>> injectableFields;
			public readonly IEnumerable<InjectableMemberInfo<PropertyInfo>> injectableProperties;
			public readonly IEnumerable<InjectableMemberInfo<MethodInfo>> injectableMethods;

			public TypeInjectionInfo(Type type)
			{
				this.type = type;

				this.injectableConstructors = type.
					GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).
					Where(c => c.IsDefined(typeof(InjectAttribute), false)).
					Select(m => new InjectableMemberInfo<ConstructorInfo>(m, m.GetCustomAttributes<InjectAttribute>(false)));

				this.injectableFields = type.
					GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).
					Where(f => f.IsDefined(typeof(InjectAttribute), false) && !f.IsLiteral).    // Exclude constants.
					Select(f => new InjectableMemberInfo<FieldInfo>(f, f.GetCustomAttributes<InjectAttribute>(false)));

				this.injectableProperties = type.
					GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).
					Where(f => f.IsDefined(typeof(InjectAttribute), false) && f.CanWrite).  // Exclude properties we can't write to.;
					Select(p => new InjectableMemberInfo<PropertyInfo>(p, p.GetCustomAttributes<InjectAttribute>(false)));

				this.injectableMethods = type.
					GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).
					Where(f => f.IsDefined(typeof(InjectAttribute), false)).
					Select(m => new InjectableMemberInfo<MethodInfo>(m, m.GetCustomAttributes<InjectAttribute>(false)));
			}
		}

		private struct InjectableMemberInfo<T>
		where T : MemberInfo
		{
			public readonly T member;
			public readonly HashSet<string> definedScopes;

			public InjectableMemberInfo(T member, IEnumerable<InjectAttribute> attributes)
			{
				member.ThrowIfNull(nameof(member));
				attributes.ThrowIfNull(nameof(attributes));

				this.member = member;
				this.definedScopes = new HashSet<string>();
				foreach (InjectAttribute injectAttr in attributes)
				{
					if (injectAttr != null)
					{
						this.definedScopes.Add((injectAttr.InjectID != null) ? injectAttr.InjectID : string.Empty);
					}
				}
			}

			public bool IsInjectionScopeDefined(string scopeName)
			{
				if (scopeName == null)
				{
					scopeName = string.Empty;
				}

				return definedScopes.Contains(scopeName);
			}
		}
	}
}
