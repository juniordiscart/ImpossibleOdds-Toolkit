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

		public string TranslationKey
		{
			get; set;
		}
	}

	public static class EnumExtensions
	{
		private static Dictionary<FieldInfo, DisplayNameAttribute> cache = new Dictionary<FieldInfo, DisplayNameAttribute>();

		/// Retrieve the display name for the given enum value.
		/// If no display name is available, then the value returned
		/// is the same as using the ToString() method.
		public static string DisplayName(this Enum e)
		{
			DisplayNameAttribute attr = GetAttributeFromEnum(e);
			return ((attr != null) && (attr.Name != null)) ? attr.Name : e.ToString();
		}

		/// Retrieve the translation key for the given enum value.
		/// If no display name is available, then an empty string is returned.
		public static string TranslationKey(this Enum e)
		{
			DisplayNameAttribute attr = GetAttributeFromEnum(e);
			return ((attr != null) && (attr.TranslationKey != null)) ? attr.TranslationKey : string.Empty;
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
