namespace ImpossibleOdds.Json
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class JsonObjectAttribute : Attribute, ILookupDataStructure
	{ }
}
