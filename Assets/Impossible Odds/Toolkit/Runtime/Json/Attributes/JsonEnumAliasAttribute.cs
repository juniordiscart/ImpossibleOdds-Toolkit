namespace ImpossibleOdds.Json
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class JsonEnumAliasAttribute : Attribute, IEnumAliasParameter
	{
		private readonly string alias = null;

		public string Alias
		{
			get => alias;
		}

		public JsonEnumAliasAttribute(string alias)
		{
			alias.ThrowIfNullOrEmpty(nameof(alias));
			this.alias = alias;
		}
	}
}
