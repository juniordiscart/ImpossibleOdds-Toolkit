namespace ImpossibleOdds.Xml
{
	using System;

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public sealed class XmlAttributeAttribute : AbstractXmlMemberAttribute
	{
		public XmlAttributeAttribute()
		{ }

		public XmlAttributeAttribute(string key)
		: base(key)
		{ }
	}
}
