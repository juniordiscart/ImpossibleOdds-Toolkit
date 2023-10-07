using System;
using System.Reflection;
using ImpossibleOdds.ReflectionCaching;

namespace ImpossibleOdds
{
	internal struct DisplayNameCacheEntry : IMemberAttributePair
	{
		private readonly FieldInfo field;

		public DisplayNameCacheEntry(FieldInfo enumField, DisplayNameAttribute displayNameAttribute)
		{
			enumField.ThrowIfNull(nameof(enumField));
			displayNameAttribute.ThrowIfNull(nameof(displayNameAttribute));

			field = enumField;
			Attribute = displayNameAttribute;
		}

		/// <inheritdoc />
		MemberInfo IMemberAttributePair.Member => field;

		/// <inheritdoc />
		Attribute IMemberAttributePair.Attribute => Attribute;

		/// <inheritdoc />
		Type IMemberAttributePair.AttributeType => Attribute.GetType();

		/// <inheritdoc />
		public DisplayNameAttribute Attribute { get; }
	}
}