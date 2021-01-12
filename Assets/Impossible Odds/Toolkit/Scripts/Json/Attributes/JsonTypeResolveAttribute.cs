﻿namespace ImpossibleOdds.Json
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
	public class JsonTypeResolveAttribute : Attribute, ILookupTypeResolveParameter
	{
		private Type target = null;
		private string value = null;

		public Type Target
		{
			get { return target; }
		}

		object ILookupTypeResolveParameter.Value
		{
			get { return Value; }
		}

		public string Value
		{
			get { return string.IsNullOrEmpty(value) ? target.Name : value; }
			set { this.value = value; }
		}

		public JsonTypeResolveAttribute(Type target)
		{
			target.ThrowIfNull(nameof(target));
			this.target = target;
		}
	}
}
