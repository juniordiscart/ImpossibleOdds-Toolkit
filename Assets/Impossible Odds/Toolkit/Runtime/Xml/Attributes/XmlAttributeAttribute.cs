using System;

namespace ImpossibleOdds.Xml
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class XmlAttributeAttribute : AbstractXmlMemberAttribute
	{
		public XmlAttributeAttribute()
		{ }

		public XmlAttributeAttribute(string key)
		: base(key)
		{ }
	}
}