namespace ImpossibleOdds.Photon.WebRpc
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class WebRpcEnumAliasAttribute : Attribute, IEnumAliasParameter
	{
		private readonly string alias = null;

		public string Alias
		{
			get => alias;
		}

		public WebRpcEnumAliasAttribute(string alias)
		{
			alias.ThrowIfNullOrEmpty(nameof(alias));
			this.alias = alias;
		}
	}
}
