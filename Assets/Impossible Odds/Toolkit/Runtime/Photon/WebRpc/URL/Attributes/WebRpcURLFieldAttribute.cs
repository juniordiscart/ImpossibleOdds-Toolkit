namespace ImpossibleOdds.Photon.WebRpc
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class WebRpcUrlFieldAttribute : Attribute, ILookupParameter<string>
	{
		private string key = null;

		object ILookupParameter.Key
		{
			get => Key;
		}

		public string Key
		{
			get => key;
			set => key = value;
		}

		public WebRpcUrlFieldAttribute()
		{ }

		public WebRpcUrlFieldAttribute(string key)
		{
			this.key = key;
		}
	}
}
