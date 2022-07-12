namespace ImpossibleOdds.DependencyInjection
{
	using System;
	using System.Reflection;
	using ImpossibleOdds.ReflectionCaching;

	internal class MemberInjectionValue<TMemberInfo> : IMemberInjectionValue
	where TMemberInfo : MemberInfo
	{
		private readonly InjectAttribute injectAttribute = null;
		private readonly TMemberInfo member = null;

		public MemberInjectionValue(TMemberInfo member, InjectAttribute injectAttribute)
		{
			member.ThrowIfNull(nameof(member));
			injectAttribute.ThrowIfNull(nameof(injectAttribute));
			this.member = member;
			this.injectAttribute = injectAttribute;
		}

		/// <inheritdoc />
		public TMemberInfo Member
		{
			get => member;
		}

		/// <inheritdoc />
		public InjectAttribute Attribute
		{
			get => injectAttribute;
		}

		/// <inheritdoc />
		public Type AttributeType
		{
			get => typeof(InjectAttribute);
		}

		/// <inheritdoc />
		MemberInfo IMemberAttributePair.Member
		{
			get => Member;
		}

		/// <inheritdoc />
		Attribute IMemberAttributePair.Attribute
		{
			get => injectAttribute;
		}
	}
}
