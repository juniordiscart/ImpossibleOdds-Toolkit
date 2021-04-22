namespace ImpossibleOdds.Xml
{
	using System;

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public sealed class XmlListItemAttribute : Attribute
	{
		private readonly string itemName = string.Empty;

		public string ItemName
		{
			get { return itemName; }
		}

		public XmlListItemAttribute(string itemName)
		{
			itemName.ThrowIfNullOrWhitespace(nameof(itemName));
			this.itemName = itemName;
		}
	}
}
