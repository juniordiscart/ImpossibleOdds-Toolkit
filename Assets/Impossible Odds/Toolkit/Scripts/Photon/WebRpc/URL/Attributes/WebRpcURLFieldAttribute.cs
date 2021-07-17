namespace ImpossibleOdds.Photon.WebRpc
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class WebRpcURLFieldAttribute : Attribute, ILookupParameter<string>
	{
		private string key = null;

		object ILookupParameter.Key
		{
			get { return Key; }
		}

		public string Key
		{
			get { return key; }
			set { key = value; }
		}

		public WebRpcURLFieldAttribute()
		{ }

		public WebRpcURLFieldAttribute(string key)
		{
			this.key = key;
		}
	}
}
