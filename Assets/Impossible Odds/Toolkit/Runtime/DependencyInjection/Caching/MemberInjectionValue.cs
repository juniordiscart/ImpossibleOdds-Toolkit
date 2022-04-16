namespace ImpossibleOdds.DependencyInjection
{
	using System;
	using System.Reflection;
	using ImpossibleOdds.ReflectionCaching;

	internal interface IMemberInjectionValue : IMemberAttributePair
	{
		new InjectAttribute Attribute
		{
			get;
		}
	}

	internal class MemberInjectionValue<TMemberInfo> : IMemberInjectionValue
	where TMemberInfo : MemberInfo
	{
		private readonly InjectAttribute injectAttribute = null;
		private readonly TMemberInfo member = null;

		public TMemberInfo Member
		{
			get => member;
		}

		public InjectAttribute Attribute
		{
			get => injectAttribute;
		}

		MemberInfo IMemberAttributePair.Member
		{
			get => Member;
		}

		Attribute IMemberAttributePair.Attribute
		{
			get => injectAttribute;
		}

		Type IMemberAttributePair.TypeOfAttribute
		{
			get => typeof(InjectAttribute);
		}

		public MemberInjectionValue(TMemberInfo member, InjectAttribute injectAttribute)
		{
			member.ThrowIfNull(nameof(member));
			injectAttribute.ThrowIfNull(nameof(injectAttribute));
			this.member = member;
			this.injectAttribute = injectAttribute;
		}
	}
}
