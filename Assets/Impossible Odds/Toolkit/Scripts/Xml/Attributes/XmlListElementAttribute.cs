namespace ImpossibleOdds.Xml
{
	using System;

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public sealed class XmlListElementAttribute : AbstractXmlMemberAttribute
	{
		private string childElementName = "Entry";

		/// <summary>
		/// The name each entry in the list should have.
		/// </summary>
		public string ListEntryName
		{
			get { return childElementName; }
			set { childElementName = value; }
		}

		public XmlListElementAttribute()
		{ }

		public XmlListElementAttribute(string key)
		: base(key)
		{ }
	}
}
