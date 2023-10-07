using System;
using System.Reflection;

namespace ImpossibleOdds.Serialization.Caching
{
	public class RequiredSerializableMember : IRequiredSerializableMember
	{
		public RequiredSerializableMember(MemberInfo member, Attribute requiredAttribute)
		{
			member.ThrowIfNull(nameof(member));
			requiredAttribute.ThrowIfNull(nameof(requiredAttribute));

			Member = member;
			Attribute = requiredAttribute;
		}

		/// <inheritdoc />
		public MemberInfo Member { get; }

		/// <inheritdoc />
		public Attribute Attribute { get; }

		/// <inheritdoc />
		public Type AttributeType => Attribute.GetType();

		/// <inheritdoc />
		public IRequiredParameter RequiredParameterAttribute => Attribute as IRequiredParameter;
	}
}