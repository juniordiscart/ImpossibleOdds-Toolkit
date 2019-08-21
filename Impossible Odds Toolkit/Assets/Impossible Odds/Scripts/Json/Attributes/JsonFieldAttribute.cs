﻿namespace ImpossibleOdds.Json
{
	using System;
	using ImpossibleOdds.DataMapping;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class JsonFieldAttribute : Attribute, ILookupParameter<string>
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

		public JsonFieldAttribute()
		{ }
	}
}
