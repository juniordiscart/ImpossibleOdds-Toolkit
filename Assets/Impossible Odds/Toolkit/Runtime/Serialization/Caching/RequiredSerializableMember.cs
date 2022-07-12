
namespace ImpossibleOdds.Serialization.Caching
{
	using System;
	using System.Reflection;

	public class RequiredSerializableMember : IRequiredSerializableMember
	{
		private readonly MemberInfo member;
		private readonly Attribute attribute;

		public RequiredSerializableMember(MemberInfo member, Attribute requiredAttribute)
		{
			member.ThrowIfNull(nameof(member));
			requiredAttribute.ThrowIfNull(nameof(requiredAttribute));

			this.member = member;
			this.attribute = requiredAttribute;
		}

		/// <inheritdoc />
		public MemberInfo Member
		{
			get => member;
		}

		/// <inheritdoc />
		public Attribute Attribute
		{
			get => attribute;
		}

		/// <inheritdoc />
		public Type AttributeType
		{
			get => attribute.GetType();
		}

		/// <inheritdoc />
		public IRequiredParameter RequiredParameterAttribute
		{
			get => attribute as IRequiredParameter;
		}
	}
}
