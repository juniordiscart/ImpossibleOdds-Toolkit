namespace ImpossibleOdds
{
	using System;
	using System.Collections.Concurrent;
	using System.Reflection;
	using ImpossibleOdds.ReflectionCaching;

	/// <summary>
	/// Caching system for enum display names.
	/// </summary>
	internal class DisplayNameCache : IReflectionMap
	{
		internal static ConcurrentDictionary<Type, DisplayNameCache> cache = new ConcurrentDictionary<Type, DisplayNameCache>();

		internal static DisplayNameAttribute GetAttributeFromEnum(Enum e)
		{
			Type enumType = e.GetType();
			if (!cache.ContainsKey(enumType))
			{
				cache.TryAdd(enumType, new DisplayNameCache(enumType));
			}

			return cache[enumType][e];
		}

		private readonly Type enumType;
		private ConcurrentDictionary<Enum, DisplayNameCacheEntry> displayNameFields = null;

		public DisplayNameCache(Type type)
		{
			type.ThrowIfNull(nameof(type));

			if (!type.IsEnum)
			{
				throw new ReflectionCachingException("The provided type {0} is not an enum.", type.Name);
			}

			this.enumType = type;

			displayNameFields = new ConcurrentDictionary<Enum, DisplayNameCacheEntry>();
			foreach (Enum enumValue in Enum.GetValues(type))
			{
				FieldInfo enumField = type.GetField(enumValue.ToString());
				if (Attribute.IsDefined(enumField, typeof(DisplayNameAttribute)))
				{
					displayNameFields.TryAdd(enumValue, new DisplayNameCacheEntry(enumField, enumField.GetCustomAttribute<DisplayNameAttribute>()));
				}
			}
		}

		/// <inheritdoc />
		public Type Type
		{
			get => enumType;
		}

		public DisplayNameAttribute this[Enum enumValue]
		{
			get => displayNameFields.ContainsKey(enumValue) ? displayNameFields[enumValue].Attribute : null;
		}
	}
}
