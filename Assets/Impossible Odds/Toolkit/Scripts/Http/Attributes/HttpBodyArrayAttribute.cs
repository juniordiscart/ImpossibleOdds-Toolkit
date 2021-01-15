namespace ImpossibleOdds.Http
{
	using System;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class HttpBodyArrayAttribute : Attribute
	{ }
}
