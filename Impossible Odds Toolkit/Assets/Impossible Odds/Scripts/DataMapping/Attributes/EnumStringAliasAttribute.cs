namespace ImpossibleOdds.DataMapping
{
	using System;
	
	/// <summary>
	/// Provides an string-based alias value for the enum value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class EnumStringAliasAttribute : Attribute
	{
		public readonly string aliasValue = string.Empty;

		public EnumStringAliasAttribute(string aliasValue)
		{
			if (string.IsNullOrEmpty(aliasValue))
			{
				throw new ArgumentException("The alias value should not be null or empty.");
			}

			this.aliasValue = aliasValue;
		}
	}
}