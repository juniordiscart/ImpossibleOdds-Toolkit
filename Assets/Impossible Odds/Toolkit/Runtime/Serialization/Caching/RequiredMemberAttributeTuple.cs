namespace ImpossibleOdds.Serialization.Caching
{
	using System;
	using System.Reflection;

	public struct RequiredMemberAttributeTuple : IRequiredMemberAttributeTuple
	{
		private readonly MemberInfo member;
		private readonly IRequiredParameter parameter;

		/// <inheritdoc />
		public MemberInfo Member
		{
			get { return member; }
		}

		/// <inheritdoc />
		public IRequiredParameter RequiredParameterAttribute
		{
			get { return parameter; }
		}

		/// <inheritdoc />
		public Type RequiredParameterAttributeType
		{
			get { return parameter.GetType(); }
		}

		public RequiredMemberAttributeTuple(MemberInfo member, IRequiredParameter parameter)
		{
			member.ThrowIfNull(nameof(member));
			parameter.ThrowIfNull(nameof(parameter));

			if (!(parameter is Attribute))
			{
				throw new SerializationException("The parameter {0} is not an attribute.", nameof(parameter));
			}

			this.member = member;
			this.parameter = parameter;
		}
	}
}
