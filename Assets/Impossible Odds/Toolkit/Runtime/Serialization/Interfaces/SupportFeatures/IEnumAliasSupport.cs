namespace ImpossibleOdds.Serialization
{
	using System;

	public interface IEnumAliasSupport
	{
		Type EnumAsStringAttributeType
		{
			get;
		}

		Type EnumAliasValueAttributeType
		{
			get;
		}
	}

	public interface IEnumAliasSupport<TEnumAsString, TEnumAliasValue> : IEnumAliasSupport
	where TEnumAsString : Attribute
	where TEnumAliasValue : Attribute, IEnumAliasParameter
	{ }
}
