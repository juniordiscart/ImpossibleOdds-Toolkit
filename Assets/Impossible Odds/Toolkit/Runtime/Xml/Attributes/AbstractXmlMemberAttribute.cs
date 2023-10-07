using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Xml
{
	public abstract class AbstractXmlMemberAttribute : Attribute, ILookupParameter<string>
	{
		public string Key { get; set; } = string.Empty;

		object ILookupParameter.Key => Key;

		public AbstractXmlMemberAttribute()
		{ }

		public AbstractXmlMemberAttribute(string key)
		{
			Key = key;
		}
	}
}