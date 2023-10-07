using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Http
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class HttpHeaderFieldAttribute : Attribute, ILookupParameter<string>
	{
		/// <inheritdoc />
		object ILookupParameter.Key => Key;

		public string Key { get; set; }

		public HttpHeaderFieldAttribute()
		{ }

		public HttpHeaderFieldAttribute(string key)
		{
			Key = key;
		}
	}
}