using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Photon.WebRpc
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class WebRpcEnumAliasAttribute : Attribute, IEnumAliasParameter
	{
		public string Alias { get; }

		public WebRpcEnumAliasAttribute(string alias)
		{
			alias.ThrowIfNullOrEmpty(nameof(alias));
			Alias = alias;
		}
	}
}