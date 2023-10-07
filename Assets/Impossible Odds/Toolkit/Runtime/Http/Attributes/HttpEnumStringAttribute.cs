using System;

namespace ImpossibleOdds.Http
{
	[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
	public sealed class HttpEnumStringAttribute : Attribute
	{ }
}