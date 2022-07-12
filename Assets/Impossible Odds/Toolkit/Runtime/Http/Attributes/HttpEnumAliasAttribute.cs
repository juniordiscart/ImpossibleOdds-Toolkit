namespace ImpossibleOdds.Http
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class HttpEnumAliasAttribute : Attribute, IEnumAliasParameter
	{
		private readonly string alias = null;

		public string Alias
		{
			get => alias;
		}

		public HttpEnumAliasAttribute(string alias)
		{
			alias.ThrowIfNullOrEmpty(nameof(alias));
			this.alias = alias;
		}
	}
}
