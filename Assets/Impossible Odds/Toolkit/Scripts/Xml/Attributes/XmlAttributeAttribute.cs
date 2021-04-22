namespace ImpossibleOdds.Xml
{
	using System;

	/// <summary>
	/// Apply the field as an attribute its XML element.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public sealed class XmlAttributeAttribute : Attribute
	{
		private readonly string attributeName = string.Empty;

		public string AttributeName
		{
			get { return attributeName; }
		}

		public XmlAttributeAttribute(string attributeName)
		{
			attributeName.ThrowIfNullOrWhitespace(nameof(attributeName));
			this.attributeName = attributeName;
		}
	}
}
