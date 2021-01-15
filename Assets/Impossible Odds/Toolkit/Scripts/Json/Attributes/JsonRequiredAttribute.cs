namespace ImpossibleOdds.Json
{
	using System;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class JsonRequiredAttribute : Attribute
	{ }
}
