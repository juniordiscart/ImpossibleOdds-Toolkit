namespace ImpossibleOdds
{
	using System;
	using System.Reflection;
	using ImpossibleOdds.ReflectionCaching;

	internal struct DisplayNameCacheEntry : IMemberAttributePair
	{
		private readonly FieldInfo field;
		private readonly DisplayNameAttribute displayNameAttribute;

		public DisplayNameCacheEntry(FieldInfo enumField, DisplayNameAttribute displayNameAttribute)
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
		Type IMemberAttributePair.AttributeType
		{
			get => displayNameAttribute.GetType();
		}

		/// <inheritdoc />
		public DisplayNameAttribute Attribute
		{
			get => displayNameAttribute;
		}
	}
}
