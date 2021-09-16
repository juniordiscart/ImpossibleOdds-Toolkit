namespace ImpossibleOdds.Http
{
	using System;

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
	public sealed class HttpBodyArrayAttribute : Attribute
	{ }
}
