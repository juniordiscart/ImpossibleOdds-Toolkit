using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Http
{

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public sealed class HttpBodyArrayAttribute : Attribute, ISequenceTypeObject
	{ }
}