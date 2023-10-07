using System;

namespace ImpossibleOdds.Xml
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class OnXmlDeserializedAttribute : Attribute
	{ }
}