using System;

namespace ImpossibleOdds.Json
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class OnJsonSerializedAttribute : Attribute
	{ }
}