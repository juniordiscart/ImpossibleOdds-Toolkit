namespace ImpossibleOdds.Xml
{
	using System;

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class OnXmlSerializedAttribute : Attribute
	{ }
}
