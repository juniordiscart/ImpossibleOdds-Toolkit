using System;

namespace ImpossibleOdds.Xml
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class XmlCDataAttribute : AbstractXmlMemberAttribute
	{
		public XmlCDataAttribute()
		{ }

		public XmlCDataAttribute(string key)
		: base(key)
		{ }
	}
}