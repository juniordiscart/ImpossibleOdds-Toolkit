namespace ImpossibleOdds.Http
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
	public sealed class HttpBodyObjectAttribute : Attribute, ILookupTypeObject
	{ }
}
