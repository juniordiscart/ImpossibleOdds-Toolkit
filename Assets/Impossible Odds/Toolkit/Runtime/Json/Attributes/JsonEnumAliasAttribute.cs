using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Json
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class JsonEnumAliasAttribute : Attribute, IEnumAliasParameter
	{
		/// <inheritdoc />
		public string Alias { get; }

		public JsonEnumAliasAttribute(string alias)
		{
			alias.ThrowIfNullOrEmpty(nameof(alias));
			Alias = alias;
		}
	}
}