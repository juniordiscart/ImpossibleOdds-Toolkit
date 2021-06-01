namespace ImpossibleOdds.Xml
{
	using System;

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public sealed class XmlCDATAAttribute : AbstractXmlMemberAttribute
	{
		public XmlCDATAAttribute()
		{ }

		public XmlCDATAAttribute(string key)
		: base(key)
		{ }
	}
}
