namespace ImpossibleOdds.Json
{
	using System;

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public sealed class JsonRequiredAttribute : Attribute
	{ }
}
