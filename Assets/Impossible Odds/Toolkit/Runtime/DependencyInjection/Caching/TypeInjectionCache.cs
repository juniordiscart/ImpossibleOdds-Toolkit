using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ImpossibleOdds.ReflectionCaching;

namespace ImpossibleOdds.DependencyInjection
{

	using InjectableField = MemberInjectionValue<System.Reflection.FieldInfo>;
	using InjectableProperty = MemberInjectionValue<System.Reflection.PropertyInfo>;
	using InjectableMethod = MemberInjectionValue<System.Reflection.MethodInfo>;
	using InjectableConstructor = MemberInjectionValue<System.Reflection.ConstructorInfo>;

	/// <summary>
	/// Caching system for injectable members of a type.
	/// </summary>
	internal class TypeInjectionCache : IReflectionMap
	{
		public TypeInjectionCache(Type type)
		{
			type.ThrowIfNull(nameof(type));
			Type = type;
			ScanType();
		}

		/// <inheritdoc />
		public Type Type { get; }

		/// <summary>
		/// Injectable constructors.
		/// </summary>
		public InjectableConstructor[] InjectableConstructors { get; private set; }

		/// <summary>
		/// Injectable fields.
		/// </summary>
		public InjectableField[] InjectableFields { get; private set; }

		/// <summary>
		/// Injectable properties.
		/// </summary>
		public InjectableProperty[] InjectableProperties { get; private set; }

		/// <summary>
		/// Injectable methods.
		/// </summary>
		public InjectableMethod[] InjectableMethods { get; private set; }

		private void ScanType()
		{
			const MemberTypes requestedMembers = MemberTypes.Constructor | MemberTypes.Field | MemberTypes.Property | MemberTypes.Method;
			const BindingFlags bindingFlags = TypeReflectionUtilities.DefaultBindingFlags | BindingFlags.Static;    // Include static bindings as well.
			IEnumerable<MemberInfo> injectableMembers = TypeReflectionUtilities.FindAllMembersWithAttribute(Type, typeof(InjectAttribute), false, requestedMembers, bindingFlags);

			InjectableFields =
				injectableMembers
				.Where(m => m is FieldInfo { IsLiteral: false })   // Exclude constants.
				.Cast<FieldInfo>()
				.Select(f => new InjectableField(f, f.GetCustomAttribute<InjectAttribute>(true)))
				.ToArray();

			InjectableProperties =
				injectableMembers
				.Where(m => m is PropertyInfo { CanWrite: true })  // Exclude properties we can't write to.
				.Cast<PropertyInfo>()
				.Select(p => new InjectableProperty(p, p.GetCustomAttribute<InjectAttribute>(true)))
				.ToArray();

			InjectableMethods =
				injectableMembers
				.Where(m => m is MethodInfo)
				.Cast<MethodInfo>()
				.Select(m => new InjectableMethod(m, m.GetCustomAttribute<InjectAttribute>(true)))
				.ToArray();

			InjectableConstructors =
				injectableMembers
				.Where(m => (m is ConstructorInfo c) && (c.DeclaringType == Type))
				.Cast<ConstructorInfo>()
				.Select(c => new InjectableConstructor(c, c.GetCustomAttribute<InjectAttribute>(true)))
				.ToArray();
		}
	}
}