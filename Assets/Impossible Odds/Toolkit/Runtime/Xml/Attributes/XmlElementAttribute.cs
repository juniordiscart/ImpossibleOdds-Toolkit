namespace ImpossibleOdds.Xml
{
	using System;

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public sealed class XmlElementAttribute : AbstractXmlMemberAttribute
	{
		public XmlElementAttribute()
		{ }

		public XmlElementAttribute(string key)
		: base(key)
		{ }
	}
}
