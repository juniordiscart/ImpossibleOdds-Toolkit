using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Photon.WebRpc
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class WebRpcFieldAttribute : Attribute, ILookupParameter
	{
		public object Key { get; }

		public WebRpcFieldAttribute(object key)
		{
			key.ThrowIfNull(nameof(key));
			Key = key;
		}
	}
}