namespace ImpossibleOdds.Serialization.Caching
{
	using System;
	using System.Collections.Concurrent;
	using System.Linq;
	using System.Reflection;
	using ImpossibleOdds;
	using ImpossibleOdds.ReflectionCaching;

	internal class EnumSerializationReflectionMap : IReflectionMap
	{
		private static readonly string[] EnumFlagSplit = new string[] { ", " };

		private readonly Type enumType;
		private readonly bool isFlag;
		private readonly Enum[] values;
		private readonly string[] names;

		private ConcurrentDictionary<Type, Attribute[]> typeDefinedAttributes = new ConcurrentDictionary<Type, Attribute[]>();
		private ConcurrentDictionary<Type, CachedEnumEntry[]> enumSerializableValues = new ConcurrentDictionary<Type, CachedEnumEntry[]>();

		public EnumSerializationReflectionMap(Type type)
		{
			type.ThrowIfNull(nameof(type));

			if (!type.IsEnum)
			{
				throw new ReflectionCachingException("The provided type {0} is not an enum.", type.Name);
			}

			this.enumType = type;
			this.isFlag = Attribute.IsDefined(type, typeof(FlagsAttribute), false);
			values = Enum.GetValues(type).Cast<Enum>().ToArray();
			names = Enum.GetNames(type);
		}

		/// <summary>
		/// Is this enum type decorated with the System.Flags attribute?
		/// </summary>
		public bool IsFlag
		{
			get => isFlag;
		}

		/// <inheritdoc />
		public Type Type
		{
			get => enumType;
		}

		/// <summary>
		/// The values associated with the enum.
		/// </summary>
		public Enum[] Values
		{
			get => values;
		}

		/// <summary>
		/// The original names of each enum value.
		/// </summary>
		public string[] Names
		{
			get => names;
		}

		public CachedEnumEntry[] GetEnumSerializationValues(IEnumAliasFeature enumAliasSupport)
		{
			enumAliasSupport.ThrowIfNull(nameof(enumAliasSupport));
			enumAliasSupport.AliasValueAttributeType.ThrowIfNull(nameof(enumAliasSupport.AliasValueAttributeType));
			return FindEnumAliases(enumAliasSupport.AliasValueAttributeType);
		}

		/// <summary>
		/// Does this enum prefer to be represented as an enum in this serialization context?
		/// </summary>
		public bool PrefersStringBasedRepresentation(IEnumAliasFeature enumAliasSupport)
		{
			enumAliasSupport.ThrowIfNull(nameof(enumAliasSupport));
			enumAliasSupport.AsStringAttributeType.ThrowIfNull(nameof(enumAliasSupport.AsStringAttributeType));
			return !FindTypeDefinedAttributes(enumAliasSupport.AsStringAttributeType).IsNullOrEmpty();
		}

		/// <summary>
		/// Is the value defined on this enum?
		/// </summary>
		public bool IsValueDefined(Enum value)
		{
			return Array.Exists(values, (v => v.Equals(value)));
		}

		/// <summary>
		/// Is the original name defined on this enum?
		/// </summary>
		public bool IsNameDefined(string name)
		{
			name.ThrowIfNullOrEmpty(name);
			return Array.Exists(names, n => n.Equals(name));
		}

		public string GetStringRepresentationFor(Enum value, IEnumAliasFeature enumAliasSupport)
		{
			enumAliasSupport.ThrowIfNull(nameof(enumAliasSupport));
			enumAliasSupport.AliasValueAttributeType.ThrowIfNull(nameof(enumAliasSupport.AliasValueAttributeType));

			int resultIndex = -1;
			CachedEnumEntry[] aliasEntries = FindEnumAliases(enumAliasSupport.AliasValueAttributeType);
			if (values.TryFindIndex((v => v.Equals(value)), out resultIndex))
			{
				return aliasEntries[resultIndex].StringBasedRepresentation;    // This works because the alias entries are in the same sequence as the name and value arrays.
			}

			if (IsFlag)
			{
				// Explode the current result.
				string[] enumStringValues = value.ToString().Split(EnumFlagSplit, StringSplitOptions.RemoveEmptyEntries);

				// Check each value name whether an alias is available.
				for (int i = 0; i < enumStringValues.Length; ++i)
				{
					if (names.TryFindIndex(n => n.Equals(enumStringValues[i]), out resultIndex))
					{
						enumStringValues[i] = aliasEntries[resultIndex].StringBasedRepresentation;
					}
				}

				// Stitch back together the result based on the replaced values.
				return string.Join(EnumFlagSplit[0], enumStringValues);
			}

			return value.ToString();
		}

		public Enum GetEnumValueFor(string value, IEnumAliasFeature enumAliasSupport)
		{
			enumAliasSupport.ThrowIfNull(nameof(enumAliasSupport));
			enumAliasSupport.AliasValueAttributeType.ThrowIfNull(nameof(enumAliasSupport.AliasValueAttributeType));

			int resultIndex = -1;
			CachedEnumEntry[] aliasEntries = FindEnumAliases(enumAliasSupport.AliasValueAttributeType);
			if (aliasEntries.TryFindIndex((ae => ae.alias.Equals(value)), out resultIndex))
			{
				return aliasEntries[resultIndex].value;
			}

			if (names.TryFindIndex((n => n.Equals(value)), out resultIndex))
			{
				return values[resultIndex];
			}

			if (IsFlag && value.Contains(EnumFlagSplit[0]))
			{
				string[] splitValues = value.Split(EnumFlagSplit, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < splitValues.Length; ++i)
				{
					if (aliasEntries.TryFindIndex((ae => ae.alias.Equals(splitValues[i])), out resultIndex))
					{
						splitValues[i] = aliasEntries[resultIndex].name;
					}
				}

				value = string.Join(EnumFlagSplit[0], splitValues);
			}

			return Enum.Parse(Type, value) as Enum;
		}

		private Attribute[] FindTypeDefinedAttributes(Type attributeType)
		{
			if (typeDefinedAttributes.TryGetValue(attributeType, out Attribute[] r))
			{
				return r;
			}

			Attribute[] attributes = Attribute.GetCustomAttributes(enumType, attributeType, true);
			return typeDefinedAttributes.GetOrAdd(attributeType, !attributes.IsNullOrEmpty() ? attributes : Array.Empty<Attribute>());
		}

		private CachedEnumEntry[] FindEnumAliases(Type attributeType)
		{
			if (enumSerializableValues.TryGetValue(attributeType, out CachedEnumEntry[] result))
			{
				return result;
			}

			// Get all enum values and couple them to their field and name.
			CachedEnumEntry[] enumSerializationNames = new CachedEnumEntry[Names.Length];
			for (int i = 0; i < Names.Length; ++i)
			{
				Enum value = Values[i];
				string name = Names[i];
				string alias = string.Empty;
				FieldInfo field = Type.GetField(name);  // Field can be retrieved based on the name of the value.
				Attribute attr = Attribute.GetCustomAttribute(field, attributeType);

				// If a custom alias is defined, then pick up that one as the name.
				if (attr is IEnumAliasParameter enumAliasParam && !string.IsNullOrEmpty(enumAliasParam.Alias))
				{
					alias = enumAliasParam.Alias;
				}

				enumSerializationNames[i] = new CachedEnumEntry(value, Names[i], alias);
			}

			return enumSerializableValues.GetOrAdd(attributeType, !enumSerializationNames.IsNullOrEmpty() ? enumSerializationNames : Array.Empty<CachedEnumEntry>());
		}

		internal readonly struct CachedEnumEntry
		{
			public readonly Enum value;
			public readonly string name;
			public readonly string alias;

			public CachedEnumEntry(Enum value, string originalName, string alias)
			{
				originalName.ThrowIfNullOrEmpty(nameof(originalName));
				this.value = value;
				this.name = originalName;
				this.alias = alias;
			}

			public string StringBasedRepresentation
			{
				get => !string.IsNullOrEmpty(alias) ? alias : name;
			}
		}
	}
}