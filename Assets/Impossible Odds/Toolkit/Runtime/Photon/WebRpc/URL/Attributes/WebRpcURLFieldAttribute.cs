using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Photon.WebRpc
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class WebRpcUrlFieldAttribute : Attribute, ILookupParameter<string>
	{
		object ILookupParameter.Key => Key;

		public string Key { get; set; }

		public WebRpcUrlFieldAttribute()
		{ }

		public WebRpcUrlFieldAttribute(string key)
		{
			Key = key;
		}
	}
}