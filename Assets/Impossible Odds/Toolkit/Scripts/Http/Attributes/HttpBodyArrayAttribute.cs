namespace ImpossibleOdds.Http
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class HttpBodyArrayAttribute : Attribute, IIndexDataStructure
	{ }
}
