using System;

namespace ImpossibleOdds.Json
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class OnJsonSerializingAttribute : Attribute
	{ }
}