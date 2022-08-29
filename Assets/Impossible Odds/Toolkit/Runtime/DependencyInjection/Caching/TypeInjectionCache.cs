namespace ImpossibleOdds.DependencyInjection
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using ImpossibleOdds.ReflectionCaching;

	using InjectableField = MemberInjectionValue<System.Reflection.FieldInfo>;
	using InjectableProperty = MemberInjectionValue<System.Reflection.PropertyInfo>;
	using InjectableMethod = MemberInjectionValue<System.Reflection.MethodInfo>;
	using InjectableConstructor = MemberInjectionValue<System.Reflection.ConstructorInfo>;

	/// <summary>
	/// Caching system for injectable members of a type.
	/// </summary>
	internal class TypeInjectionCache : IReflectionMap
	{
		private readonly Type type;
		private InjectableField[] injectableFields = null;
		private InjectableProperty[] injectableProperties = null;
		private InjectableMethod[] injectableMethods = null;
		private InjectableConstructor[] injectableConstructors = null;

		public TypeInjectionCache(Type type)
		{
			type.ThrowIfNull(nameof(type));
			this.type = type;
			ScanType();
		}

		/// <inheritdoc />
		public Type Type
		{
			get => type;
		}

		/// <summary>
		/// Injectable constructors.
		/// </summary>
		public InjectableConstructor[] InjectableConstructors
		{
			get => injectableConstructors;
		}

		/// <summary>
		/// Injectable fields.
		/// </summary>
		public InjectableField[] InjectableFields
		{
			get => injectableFields;
		}

		/// <summary>
		/// Injectable properties.
		/// </summary>
		public InjectableProperty[] InjectableProperties
		{
			get => injectableProperties;
		}

		/// <summary>
		/// Injectable methods.
		/// </summary>
		public InjectableMethod[] InjectableMethods
		{
			get => injectableMethods;
		}

		private void ScanType()
		{
			const MemberTypes requestedMembers = MemberTypes.Constructor | MemberTypes.Field | MemberTypes.Property | MemberTypes.Method;
			const BindingFlags bindingFlags = TypeReflectionUtilities.DefaultBindingFlags | BindingFlags.Static;    // Include static bindings as well.
			IEnumerable<MemberInfo> injectableMembers = TypeReflectionUtilities.FindAllMembersWithAttribute(Type, typeof(InjectAttribute), false, requestedMembers, bindingFlags);

			injectableFields =
				injectableMembers
				.Where(m => m is FieldInfo f && !f.IsLiteral)   // Exclude constants.
				.Cast<FieldInfo>()
				.Select(f => new InjectableField(f, f.GetCustomAttribute<InjectAttribute>(true)))
				.ToArray();

			injectableProperties =
				injectableMembers
				.Where(m => m is PropertyInfo p && p.CanWrite)  // Exclude properties we can't write to.
				.Cast<PropertyInfo>()
				.Select(p => new InjectableProperty(p, p.GetCustomAttribute<InjectAttribute>(true)))
				.ToArray();

			injectableMethods =
				injectableMembers
				.Where(m => m is MethodInfo)
				.Cast<MethodInfo>()
				.Select(m => new InjectableMethod(m, m.GetCustomAttribute<InjectAttribute>(true)))
				.ToArray();

			injectableConstructors =
				injectableMembers
				.Where(m => m is ConstructorInfo)
				.Cast<ConstructorInfo>()
				.Select(c => new InjectableConstructor(c, c.GetCustomAttribute<InjectAttribute>(true)))
				.ToArray();
		}
	}
}
