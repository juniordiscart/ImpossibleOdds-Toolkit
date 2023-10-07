using System;

namespace ImpossibleOdds.Xml
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class XmlElementAttribute : AbstractXmlMemberAttribute
	{
		public XmlElementAttribute()
		{ }

		public XmlElementAttribute(string key)
		: base(key)
		{ }
	}
}