using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

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

	public static class EnumExtensions
	{
		private static Dictionary<FieldInfo, DisplayNameAttribute> cache = new Dictionary<FieldInfo, DisplayNameAttribute>();

		/// <summary>
		/// Retrieve the display name for the given enum value.
		/// </summary>
		/// <param name="e">Enum value</param>
		/// <returns>The display name for the enum value. If no display name is defined, the result of ToString() is returned.</returns>
		public static string DisplayName(this Enum e)
		{
			DisplayNameAttribute attr = GetAttributeFromEnum(e);
			return ((attr != null) && (attr.Name != null)) ? attr.Name : e.ToString();
		}

		/// <summary>
		/// Retrieve the localization key for the given enum value.
		/// </summary>
		/// <param name="e">Enum value</param>
		/// <returns>The localization key for the enum value. If no localization key is defined, an empty string is returned.</returns>
		public static string LocalizationKey(this Enum e)
		{
			DisplayNameAttribute attr = GetAttributeFromEnum(e);
			return ((attr != null) && (attr.LocalizationKey != null)) ? attr.LocalizationKey : string.Empty;
		}

		private static DisplayNameAttribute GetAttributeFromEnum(Enum e)
		{
			FieldInfo field = e.GetType().GetField(e.ToString());
			DisplayNameAttribute attr = null;
			if (cache.ContainsKey(field))
			{
				attr = cache[field];
			}
			else
			{
				attr = GetAtttributeFromField(field);
				cache.Add(field, attr);
			}

			return attr;
		}

		private static DisplayNameAttribute GetAtttributeFromField(FieldInfo field)
		{
			return field.GetCustomAttributes(typeof(DisplayNameAttribute), false).FirstOrDefault() as DisplayNameAttribute;
		}
	}
}
