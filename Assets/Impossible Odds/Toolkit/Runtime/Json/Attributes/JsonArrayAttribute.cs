using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Json
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public sealed class JsonArrayAttribute : Attribute, ISequenceTypeObject
	{ }
}