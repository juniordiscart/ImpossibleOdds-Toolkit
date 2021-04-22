namespace ImpossibleOdds.Xml
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class XmlFieldAttribute : Attribute, ILookupParameter<string>
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

		public XmlFieldAttribute()
		{ }

		public XmlFieldAttribute(string key)
		{
			this.key = key;
		}
	}
}
