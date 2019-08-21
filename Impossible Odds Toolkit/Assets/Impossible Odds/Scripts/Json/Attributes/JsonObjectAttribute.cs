namespace ImpossibleOdds.Json
{
	using System;
	using ImpossibleOdds.DataMapping;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class JsonObjectAttribute : Attribute, ILookupDataStructure
	{ }
}
