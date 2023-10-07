using System;
using System.Reflection;
using ImpossibleOdds.ReflectionCaching;

namespace ImpossibleOdds.DependencyInjection
{
	internal struct MemberInjectionValue<TMemberInfo> : IMemberInjectionValue
	where TMemberInfo : MemberInfo
	{
		public MemberInjectionValue(TMemberInfo member, InjectAttribute injectAttribute)
		{
			member.ThrowIfNull(nameof(member));
			injectAttribute.ThrowIfNull(nameof(injectAttribute));
			this.Member = member;
			this.Attribute = injectAttribute;
		}

		/// <inheritdoc />
		public TMemberInfo Member { get; }

		/// <inheritdoc />
		public InjectAttribute Attribute { get; }

		/// <inheritdoc />
		public Type AttributeType => typeof(InjectAttribute);

		/// <inheritdoc />
		MemberInfo IMemberAttributePair.Member => Member;

		/// <inheritdoc />
		Attribute IMemberAttributePair.Attribute => Attribute;
	}
}