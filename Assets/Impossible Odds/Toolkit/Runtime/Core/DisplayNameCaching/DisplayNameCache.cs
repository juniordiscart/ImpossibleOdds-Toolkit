namespace ImpossibleOdds
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using ImpossibleOdds.ReflectionCaching;

	internal struct DisplayNamePair : IMemberAttributePair
	{
		private readonly FieldInfo field;
		private readonly DisplayNameAttribute displayNameAttribute;

		public DisplayNamePair(FieldInfo enumField, DisplayNameAttribute displayNameAttribute)
		{
			enumField.ThrowIfNull(nameof(enumField));
			displayNameAttribute.ThrowIfNull(nameof(displayNameAttribute));

			this.field = enumField;
			this.displayNameAttribute = displayNameAttribute;
		}

		/// <inheritdoc />
		MemberInfo IMemberAttributePair.Member
		{
			get => field;
		}

		/// <inheritdoc />
		Attribute IMemberAttributePair.Attribute
		{
			get => displayNameAttribute;
		}

		/// <inheritdoc />
		Type IMemberAttributePair.TypeOfAttribute
		{
			get => displayNameAttribute.GetType();
		}

		public DisplayNameAttribute Attribute
		{
			get => displayNameAttribute;
		}
	}

	internal class DisplayNameCache : TypeReflectionMap<DisplayNamePair>
	{
		internal static Dictionary<Type, DisplayNameCache> cache = new Dictionary<Type, DisplayNameCache>();

		internal static DisplayNameAttribute GetAttributeFromEnum(Enum e)
		{
			Type enumType = e.GetType();
			if (!cache.ContainsKey(enumType))
			{
				cache[enumType] = new DisplayNameCache(enumType);
			}

			return cache[enumType][e];
		}

		private Dictionary<Enum, DisplayNamePair> displayNameFields = null;

		public DisplayNameCache(Type type)
		: base(type)
		{
			if (!type.IsEnum)
			{
				throw new ImpossibleOddsException("Type {0} is not an enum.", type.Name);
			}

			Array enumValues = Enum.GetValues(type);
			displayNameFields = new Dictionary<Enum, DisplayNamePair>(enumValues.Length);

			foreach (Enum enumValue in enumValues)
			{
				FieldInfo enumField = type.GetField(enumValue.ToString());
				if (enumField.IsDefined(typeof(DisplayNameAttribute)))
				{
					displayNameFields[enumValue] = new DisplayNamePair(enumField, enumField.GetCustomAttribute<DisplayNameAttribute>());
				}
			}
		}

		public DisplayNameAttribute this[Enum enumValue]
		{
			get => displayNameFields.ContainsKey(enumValue) ? displayNameFields[enumValue].Attribute : null;
		}

		/// <inheritdoc />
		public override IEnumerable<DisplayNamePair> MemberAttributePairs
		{
			get => displayNameFields.Values;
		}
	}

}
