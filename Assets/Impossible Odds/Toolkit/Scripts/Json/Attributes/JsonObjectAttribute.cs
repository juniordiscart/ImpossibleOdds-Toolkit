namespace ImpossibleOdds.Json
{
	using System;

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
	public sealed class JsonObjectAttribute : Attribute
	{ }
}
