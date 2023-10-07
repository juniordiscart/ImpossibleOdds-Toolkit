using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Http
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class HttpURLFieldAttribute : Attribute, ILookupParameter<string>
	{
		object ILookupParameter.Key => Key;

		public string Key { get; set; }

		public HttpURLFieldAttribute()
		{ }

		public HttpURLFieldAttribute(string key)
		{
			Key = key;
		}
	}
}