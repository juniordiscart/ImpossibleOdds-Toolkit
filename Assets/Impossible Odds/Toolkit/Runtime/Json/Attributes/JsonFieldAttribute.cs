namespace ImpossibleOdds.Json
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public sealed class JsonFieldAttribute : Attribute, ILookupParameter<string>
	{
		private string key = null;

		/// <inheritdoc />
		object ILookupParameter.Key
		{
			get => Key;
		}

		/// <inheritdoc />
		public string Key
		{
			get => key;
			set => key = value;
		}

		public JsonFieldAttribute()
		{ }

		public JsonFieldAttribute(string key)
		{
			this.key = key;
		}
	}
}
