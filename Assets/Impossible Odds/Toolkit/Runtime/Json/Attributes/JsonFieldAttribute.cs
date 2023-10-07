using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Json
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class JsonFieldAttribute : Attribute, ILookupParameter<string>
	{
		/// <inheritdoc />
		object ILookupParameter.Key => Key;

		/// <inheritdoc />
		public string Key { get; set; }

		public JsonFieldAttribute()
		{ }

		public JsonFieldAttribute(string key)
		{
			Key = key;
		}
	}
}