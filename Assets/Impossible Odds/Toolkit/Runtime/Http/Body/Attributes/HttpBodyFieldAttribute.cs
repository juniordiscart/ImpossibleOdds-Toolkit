using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Http
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class HttpBodyFieldAttribute : Attribute, ILookupParameter<string>
	{
		object ILookupParameter.Key => Key;

		public string Key { get; }

		public HttpBodyFieldAttribute()
		{ }

		public HttpBodyFieldAttribute(string key)
		{
			Key = key;
		}
	}
}