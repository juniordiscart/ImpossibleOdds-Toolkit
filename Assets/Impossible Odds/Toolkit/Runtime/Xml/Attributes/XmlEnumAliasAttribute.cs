namespace ImpossibleOdds.Xml
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class XmlEnumAliasAttribute : Attribute, IEnumAliasParameter
	{
		private readonly string alias = null;

		public string Alias
		{
			get => alias;
		}

		public XmlEnumAliasAttribute(string alias)
		{
			alias.ThrowIfNullOrEmpty(nameof(alias));
			this.alias = alias;
		}
	}
}
