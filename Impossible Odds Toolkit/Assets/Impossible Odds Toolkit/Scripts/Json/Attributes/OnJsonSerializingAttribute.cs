namespace ImpossibleOdds.Json
{
	using System;

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class OnJsonSerializingAttribute : Attribute
	{ }
}
