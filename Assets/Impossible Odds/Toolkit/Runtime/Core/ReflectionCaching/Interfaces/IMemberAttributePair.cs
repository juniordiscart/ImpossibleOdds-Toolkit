namespace ImpossibleOdds.ReflectionCaching
{
	using System;
	using System.Reflection;

	public interface IMemberAttributePair
	{
		MemberInfo Member
		{
			get;
		}

		Attribute Attribute
		{
			get;
		}

		Type TypeOfAttribute
		{
			get;
		}
	}
}
