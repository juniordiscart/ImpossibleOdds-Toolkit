namespace ImpossibleOdds.Json
{
	using System;
	using ImpossibleOdds.DataMapping;

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
	public class JsonTypeResolveAttribute : Attribute, ILookupTypeResolveParameter
	{
		public const string TypeKey = "xsi:type";

		private Type target = null;
		private string key = null;
		private string value = null;

		public Type Target
		{
			get { return target; }
		}

		object ILookupTypeResolveParameter.Key
		{
			get { return Key; }
		}

		object ILookupTypeResolveParameter.Value
		{
			get { return Value; }
		}

		public string Key
		{
			get { return string.IsNullOrEmpty(key) ? TypeKey : key; }
			set { key = value; }
		}

		public string Value
		{
			get { return value; }
			set { this.value = value; }
		}

		public JsonTypeResolveAttribute(Type target)
		{
			target.ThrowIfNull(nameof(target));
			this.target = target;
		}
	}
}
