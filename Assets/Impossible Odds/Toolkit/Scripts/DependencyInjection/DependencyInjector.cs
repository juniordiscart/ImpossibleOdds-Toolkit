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

		private static void ResolveDependenciesForObject(object objToInject, Type currentType, IDependencyContainer container, string injectionID = null)
		{
			// Don't inject on a null type, or an object type.
			if ((currentType == null) || (currentType == ObjectType))
			{
				return;
			}

			// Inject the base types first, like we do with constructors
			ResolveDependenciesForObject(objToInject, currentType.BaseType, container, injectionID);

			// Don't bother if the type is not injectable
			if (!IsInjectable(currentType))
			{
				return;
			}

			TypeInjectionInfo injectionInfo = GetTypeInjectionInfo(currentType);

			// Fields
			foreach (Pair<FieldInfo> field in injectionInfo.injectableFields)
			{
				Type fieldType = field.member.FieldType;
				if (InjectionIDsMatch(field.attribute, injectionID) && container.BindingExists(fieldType))
				{
					field.member.SetValue(objToInject, container.GetBinding(fieldType).GetInstance());
				}
			}

			// Properties
			foreach (Pair<PropertyInfo> property in injectionInfo.injectableProperties)
			{
				Type propertyType = property.member.PropertyType;
				if (InjectionIDsMatch(property.attribute, injectionID) && container.BindingExists(propertyType))
				{
					property.member.SetValue(objToInject, container.GetBinding(propertyType).GetInstance());
				}
			}

			// Methods
			foreach (Pair<MethodInfo> method in injectionInfo.injectableMethods)
			{
				if (!InjectionIDsMatch(method.attribute, injectionID))
				{
					continue;
				}

				ParameterInfo[] parameterInfo = method.member.GetParameters();
				object[] parameters = GetParameterInjectionList(parameterInfo.Length);

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
						parameters[i] = parameterType.GetTypeInfo().IsValueType ? Activator.CreateInstance(parameterType, true) : null;
					}
				}

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

		private static TypeInjectionInfo GetTypeInjectionInfo(Type type)
		{
			if (!typeInjectionCache.ContainsKey(type))
			{
				TypeInjectionInfo injectionCache = ConstructTypeInjectables(type);
				typeInjectionCache.Add(type, injectionCache);
			}

			return typeInjectionCache[type];
		}

		private static bool InjectionIDsMatch(InjectAttribute attr, string injectionID)
		{
			bool hasInjectionIDSet = !string.IsNullOrEmpty(injectionID);
			return (!hasInjectionIDSet && !attr.HasInjectionIDSet) || hasInjectionIDSet && attr.HasInjectionIDSet && string.Equals(injectionID, attr.InjectID);
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

		/// <summary>
		/// Retrieve all fields, properties and methods that have been marked as injectable.
		/// </summary>
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

			IEnumerable<Pair<FieldInfo>> fieldPairs = injectableFields.Select(f => new Pair<FieldInfo>(f.GetCustomAttribute<InjectAttribute>(false), f));
			IEnumerable<Pair<PropertyInfo>> propertyPairs = injectableProperties.Select(p => new Pair<PropertyInfo>(p.GetCustomAttribute<InjectAttribute>(false), p));
			IEnumerable<Pair<MethodInfo>> methodPairs = injectableMethods.Select(m => new Pair<MethodInfo>(m.GetCustomAttribute<InjectAttribute>(false), m));

			TypeInjectionInfo typeCache = new TypeInjectionInfo(type, fieldPairs, propertyPairs, methodPairs);
			return typeCache;
		}

		private class TypeInjectionInfo
		{
			public readonly Type type;
			public readonly IEnumerable<Pair<FieldInfo>> injectableFields;
			public readonly IEnumerable<Pair<PropertyInfo>> injectableProperties;
			public readonly IEnumerable<Pair<MethodInfo>> injectableMethods;

			public TypeInjectionInfo(Type type, IEnumerable<Pair<FieldInfo>> injectableFields, IEnumerable<Pair<PropertyInfo>> injectableProperties, IEnumerable<Pair<MethodInfo>> injectableMethods)
			{
				this.type = type;
				this.injectableFields = injectableFields;
				this.injectableProperties = injectableProperties;
				this.injectableMethods = injectableMethods;
			}
		}

		private struct Pair<T>
		where T : MemberInfo
		{
			public readonly InjectAttribute attribute;
			public readonly T member;

			public Pair(InjectAttribute attribute, T member)
			{
				this.attribute = attribute;
				this.member = member;
			}
		}
	}
}
