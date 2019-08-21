namespace ImpossibleOdds.Http
{
	using System;
	using ImpossibleOdds.DataMapping;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class HttpListMappingAttribute : Attribute, IIndexDataStructure
	{ }
}
