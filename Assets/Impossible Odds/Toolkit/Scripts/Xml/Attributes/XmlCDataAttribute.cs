namespace ImpossibleOdds.Xml
{
	using System;

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public sealed class XmlCDataAttribute : AbstractXmlMemberAttribute
	{
		public XmlCDataAttribute()
		{ }

		public XmlCDataAttribute(string key)
		: base(key)
		{ }
	}
}
