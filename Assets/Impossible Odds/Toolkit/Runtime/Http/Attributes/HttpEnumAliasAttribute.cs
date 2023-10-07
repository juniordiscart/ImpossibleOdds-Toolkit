using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Http
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class HttpEnumAliasAttribute : Attribute, IEnumAliasParameter
	{
		public string Alias { get; }

		public HttpEnumAliasAttribute(string alias)
		{
			alias.ThrowIfNullOrEmpty(nameof(alias));
			Alias = alias;
		}
	}
}