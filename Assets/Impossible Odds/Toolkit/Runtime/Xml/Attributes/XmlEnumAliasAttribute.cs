using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Xml
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class XmlEnumAliasAttribute : Attribute, IEnumAliasParameter
	{
		/// <inheritdoc />
		public string Alias { get; }

		public XmlEnumAliasAttribute(string alias)
		{
			alias.ThrowIfNullOrEmpty(nameof(alias));
			Alias = alias;
		}
	}
}