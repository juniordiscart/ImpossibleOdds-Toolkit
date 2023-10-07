using System;

namespace ImpossibleOdds.Xml
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class OnXmlDeserializingAttribute : Attribute
	{ }
}