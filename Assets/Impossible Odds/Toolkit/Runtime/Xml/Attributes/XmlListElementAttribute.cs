namespace ImpossibleOdds.Xml
{
	using System;

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public sealed class XmlListElementAttribute : AbstractXmlMemberAttribute
	{
		public const string DefaultListEntryName = "Entry";

		private string childElementName = DefaultListEntryName;

		/// <summary>
		/// The name each entry in the list should have.
		/// </summary>
		public string EntryName
		{
			get => childElementName;
			set => childElementName = value;
		}

		public XmlListElementAttribute()
		{ }

		public XmlListElementAttribute(string key)
		: base(key)
		{ }
	}
}
