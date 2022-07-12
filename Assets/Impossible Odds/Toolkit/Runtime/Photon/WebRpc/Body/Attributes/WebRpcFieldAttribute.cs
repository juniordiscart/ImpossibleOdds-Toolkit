namespace ImpossibleOdds.Photon.WebRpc
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class WebRpcFieldAttribute : Attribute, ILookupParameter
	{
		public object Key
		{
			get => key;
		}

		private readonly object key;

		public WebRpcFieldAttribute(object key)
		{
			key.ThrowIfNull(nameof(key));
			this.key = key;
		}
	}
}
