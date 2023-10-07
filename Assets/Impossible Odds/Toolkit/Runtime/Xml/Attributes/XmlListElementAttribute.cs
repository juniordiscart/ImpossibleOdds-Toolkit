using System;

namespace ImpossibleOdds.Xml
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class XmlListElementAttribute : AbstractXmlMemberAttribute
	{
		public const string DefaultListEntryName = "Entry";

		/// <summary>
		/// The name each entry in the list should have.
		/// </summary>
		public string EntryName { get; set; } = DefaultListEntryName;

		public XmlListElementAttribute()
		{ }

		public XmlListElementAttribute(string key)
		: base(key)
		{ }
	}
}