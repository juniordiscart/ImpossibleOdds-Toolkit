namespace ImpossibleOdds
{
	using System;

	public static class EnumExtensions
	{
		/// <summary>
		/// Retrieve the display name for the given enum value.
		/// </summary>
		/// <param name="e">Enum value</param>
		/// <returns>The display name for the enum value. If no display name is defined, the result of ToString() is returned.</returns>
		public static string DisplayName(this Enum e)
		{
			DisplayNameAttribute attr = DisplayNameCache.GetAttributeFromEnum(e);
			return ((attr != null) && (attr.Name != null)) ? attr.Name : e.ToString();
		}

		/// <summary>
		/// Retrieve the localization key for the given enum value.
		/// </summary>
		/// <param name="e">Enum value</param>
		/// <returns>The localization key for the enum value. If no localization key is defined, an empty string is returned.</returns>
		public static string LocalizationKey(this Enum e)
		{
			DisplayNameAttribute attr = DisplayNameCache.GetAttributeFromEnum(e);
			return ((attr != null) && (attr.LocalizationKey != null)) ? attr.LocalizationKey : string.Empty;
		}

		/// <summary>
		/// Clears the cache containing quick access information to the display name attribute.
		/// </summary>
		public static void ClearDisplayNameCache()
		{
			DisplayNameCache.cache.Clear();
		}
	}
}
