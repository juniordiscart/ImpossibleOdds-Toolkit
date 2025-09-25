using System;
using System.Reflection;
using ImpossibleOdds.ReflectionCaching;

namespace ImpossibleOdds.DependencyInjection
{
	internal readonly struct MemberInjectionValue<TMemberInfo> : IMemberInjectionValue
	where TMemberInfo : MemberInfo
	{
		public MemberInjectionValue(TMemberInfo member, InjectAttribute injectAttribute)
		{
			member.ThrowIfNull(nameof(member));
			injectAttribute.ThrowIfNull(nameof(injectAttribute));
			Member = member;
			Attribute = injectAttribute;
		}

		/// <summary>
		/// The member the attribute is applied to.
		/// </summary>
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