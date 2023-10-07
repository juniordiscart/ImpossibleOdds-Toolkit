using System;

namespace ImpossibleOdds
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class DisplayNameAttribute : Attribute
	{
		public string Name
		{
			get; set;
		}

		public string LocalizationKey
		{
			get; set;
		}
	}
}