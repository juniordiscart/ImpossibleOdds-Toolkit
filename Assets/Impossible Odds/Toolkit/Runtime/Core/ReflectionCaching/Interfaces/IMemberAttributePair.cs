namespace ImpossibleOdds.ReflectionCaching
{
	using System;
	using System.Reflection;

	public interface IMemberAttributePair
	{
		/// <summary>
		/// The member the attribute is applied to.
		/// </summary>
		MemberInfo Member
		{
			get;
		}

		/// <summary>
		/// The attribute applied on the member.
		/// </summary>
		Attribute Attribute
		{
			get;
		}

		/// <summary>
		/// The type of the attribute
		/// </summary>
		Type AttributeType
		{
			get;
		}
	}
}
