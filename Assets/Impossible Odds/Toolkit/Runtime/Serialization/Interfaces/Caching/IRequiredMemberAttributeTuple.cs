namespace ImpossibleOdds.Serialization.Caching
{
	using System;
	using System.Reflection;

	public interface IRequiredMemberAttributeTuple
	{
		/// <summary>
		/// The member the attribute is applied to.
		/// </summary>
		MemberInfo Member
		{
			get;
		}

		/// <summary>
		/// The attribute applied to the member that marks it as being required.
		/// </summary>
		IRequiredParameter RequiredParameterAttribute
		{
			get;
		}

		/// <summary>
		/// Type of the required parameter attribute.
		/// </summary>
		Type RequiredParameterAttributeType
		{
			get;
		}
	}
}
