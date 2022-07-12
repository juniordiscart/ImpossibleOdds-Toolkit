namespace ImpossibleOdds.Http
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class HttpURLFieldAttribute : Attribute, ILookupParameter<string>
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

		public HttpURLFieldAttribute()
		{ }

		public HttpURLFieldAttribute(string key)
		{
			this.key = key;
		}
	}
}
