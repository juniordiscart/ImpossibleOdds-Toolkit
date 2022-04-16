namespace ImpossibleOdds.DependencyInjection
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using ImpossibleOdds.ReflectionCaching;

	internal class TypeInjectionCache : TypeReflectionMap<IMemberInjectionValue>
	{
		private struct ValueRange
		{
			public int Index;
			public int Offset;
		}

		private ValueRange fields;
		private ValueRange properties;
		private ValueRange methods;
		private ValueRange constructors;
		private List<IMemberInjectionValue> injectableMembers = null;

		public TypeInjectionCache(Type type)
		: base(type)
		{
			ScanType();
		}

		/// <inheritdoc />
		public override IEnumerable<IMemberInjectionValue> MemberAttributePairs
		{
			get => injectableMembers;
		}

		/// <summary>
		/// Injectable constructors.
		/// </summary>
		public IEnumerable<MemberInjectionValue<ConstructorInfo>> InjectableConstructors
		{
			get => GetFilteredMembers<ConstructorInfo>(constructors);
		}

		/// <summary>
		/// Injectable fields.
		/// </summary>
		public IEnumerable<MemberInjectionValue<FieldInfo>> InjectableFields
		{
			get => GetFilteredMembers<FieldInfo>(fields);
		}

		/// <summary>
		/// Injectable properties.
		/// </summary>
		public IEnumerable<MemberInjectionValue<PropertyInfo>> InjectableProperties
		{
			get => GetFilteredMembers<PropertyInfo>(properties);
		}

		/// <summary>
		/// Injectable methods.
		/// </summary>
		public IEnumerable<MemberInjectionValue<MethodInfo>> InjectableMethods
		{
			get => GetFilteredMembers<MethodInfo>(methods);
		}

		private void ScanType()
		{
			injectableMembers = new List<IMemberInjectionValue>();

			// Fields
			fields.Index = injectableMembers.Count;
			injectableMembers.AddRange(Type.
				GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).
				Where(f => f.IsDefined(typeof(InjectAttribute), false) && !f.IsLiteral).    // Exclude constants.
				Select(f => new MemberInjectionValue<FieldInfo>(f, f.GetCustomAttribute<InjectAttribute>(false))));
			fields.Offset = injectableMembers.Count - fields.Index;

			// Properties
			properties.Index = injectableMembers.Count;
			injectableMembers.AddRange(Type.
				GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).
				Where(f => f.IsDefined(typeof(InjectAttribute), false) && f.CanWrite).  // Exclude properties we can't write to.;
				Select(p => new MemberInjectionValue<PropertyInfo>(p, p.GetCustomAttribute<InjectAttribute>(false))));
			properties.Offset = injectableMembers.Count - properties.Index;

			// Methods
			methods.Index = injectableMembers.Count;
			injectableMembers.AddRange(Type.
				GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).
				Where(f => f.IsDefined(typeof(InjectAttribute), false)).
				Select(m => new MemberInjectionValue<MethodInfo>(m, m.GetCustomAttribute<InjectAttribute>(false))));
			methods.Offset = injectableMembers.Count - methods.Index;

			// Constructors
			constructors.Index = injectableMembers.Count;
			injectableMembers.AddRange(Type.
				GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).
				Where(c => c.IsDefined(typeof(InjectAttribute), false)).
				Select(m => new MemberInjectionValue<ConstructorInfo>(m, m.GetCustomAttribute<InjectAttribute>(false))));
			constructors.Offset = injectableMembers.Count - constructors.Index;
		}

		private IEnumerable<MemberInjectionValue<TMemberInfo>> GetFilteredMembers<TMemberInfo>(ValueRange indices)
		where TMemberInfo : MemberInfo
		{
			if (!injectableMembers.IsNullOrEmpty() && (indices.Offset > 0))
			{
				for (int i = 0; i < indices.Offset; ++i)
				{
					yield return injectableMembers[indices.Index + i] as MemberInjectionValue<TMemberInfo>;
				}
			}
		}
	}
}
