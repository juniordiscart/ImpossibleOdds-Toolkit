namespace ImpossibleOdds.Xml
{
	using System;
	using ImpossibleOdds.Serialization;

	public abstract class AbstractXmlMemberAttribute : Attribute, ILookupParameter<string>
	{
		private string key = string.Empty;

		public string Key
		{
			get { return key; }
			set { key = value; }
		}

		object ILookupParameter.Key
		{
			get { return Key; }
		}

		public AbstractXmlMemberAttribute()
		{ }

		public AbstractXmlMemberAttribute(string key)
		{
			this.key = key;
		}
	}
}
