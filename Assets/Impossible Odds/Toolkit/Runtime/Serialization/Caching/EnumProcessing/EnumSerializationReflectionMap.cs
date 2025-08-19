using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ImpossibleOdds.ReflectionCaching;

namespace ImpossibleOdds.Serialization.Caching
{
    internal class EnumSerializationReflectionMap : IReflectionMap
    {
        private const string EnumFlagSplit = ", "; // When flags are joined when converting to string.

        private readonly ConcurrentDictionary<Type, Attribute[]> typeDefinedAttributes = new ConcurrentDictionary<Type, Attribute[]>();
        private readonly ConcurrentDictionary<Type, CachedEnumEntry[]> enumSerializableValues = new ConcurrentDictionary<Type, CachedEnumEntry[]>();

        public EnumSerializationReflectionMap(Type type)
        {
            type.ThrowIfNull(nameof(type));

            if (!type.IsEnum)
            {
                throw new ReflectionCachingException($"The provided type {type.Name} is not an enum.");
            }

            Type = type;
            IsFlag = Attribute.IsDefined(type, typeof(FlagsAttribute), false);
            Values = Enum.GetValues(type).Cast<Enum>().ToArray();
            Names = Enum.GetNames(type);

            // Create the different formatted names.
            string[] gFormatted = Values.Select(v => v.ToString("G")).ToArray();
            string[] fFormatted = Values.Select(v => v.ToString("F")).ToArray();
            string[] dFormatted = Values.Select(v => v.ToString("D")).ToArray();
            string[] xFormatted = Values.Select(v => v.ToString("X")).ToArray();
            FormattedNames = new Dictionary<string, string[]>()
            {
                { "G", gFormatted },
                { "g", gFormatted },
                { "F", fFormatted },
                { "f", fFormatted },
                { "D", dFormatted },
                { "d", dFormatted },
                { "X", xFormatted },
                { "x", xFormatted }
            };
        }

        /// <summary>
        /// Is this enum type decorated with the System.Flags attribute?
        /// </summary>
        public bool IsFlag { get; }

        /// <inheritdoc />
        public Type Type { get; }

        /// <summary>
        /// The values associated with the enum.
        /// </summary>
        public Enum[] Values { get; }

        /// <summary>
        /// The original names of each enum value.
        /// </summary>
        public string[] Names { get; }

        /// <summary>
        /// The different string values of each enum value, under the different supported formats.
        /// The keys in the dictionary represent the supported formats: G, g, F, f, D, d, X and x.
        /// </summary>
        public IReadOnlyDictionary<string, string[]> FormattedNames { get; }

        public CachedEnumEntry[] GetEnumSerializationValues(IEnumAliasFeature enumAliasSupport)
        {
            enumAliasSupport.ThrowIfNull(nameof(enumAliasSupport));
            enumAliasSupport.AliasValueAttribute.ThrowIfNull(nameof(enumAliasSupport.AliasValueAttribute));
            return FindEnumAliases(enumAliasSupport.AliasValueAttribute);
        }

        /// <summary>
        /// Does this enum prefer to be represented as an enum in this serialization context?
        /// </summary>
        public bool PrefersStringBasedRepresentation(IEnumAliasFeature enumAliasSupport)
        {
            enumAliasSupport.ThrowIfNull(nameof(enumAliasSupport));
            enumAliasSupport.AsStringAttribute.ThrowIfNull(nameof(enumAliasSupport.AsStringAttribute));
            return !FindTypeDefinedAttributes(enumAliasSupport.AsStringAttribute).IsNullOrEmpty();
        }

        /// <summary>
        /// Is the value defined on this enum?
        /// </summary>
        public bool IsValueDefined(Enum value)
        {
            return Array.Exists(Values, (v => v.Equals(value)));
        }

        /// <summary>
        /// Is the original name defined on this enum?
        /// </summary>
        public bool IsNameDefined(string name)
        {
            name.ThrowIfNullOrEmpty(name);
            return Array.Exists(Names, n => n.Equals(name));
        }

        /// <summary>
        /// Retrieves the set of formatted names for a specified format.
        /// The supported formats are: G, g, F, f, D, d, X and x.
        /// If null or an empty format string is provided, the 'G' format is used.
        /// </summary>
        /// <param name="format">The format for which to get the names.</param>
        /// <returns>A list of formatted names under the desired format.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an invalid format is provided.</exception>
        public string[] GetNamesForFormat(string format)
        {
            switch (format)
            {
                case null:
                case "":
                    return FormattedNames["G"];
                default:
                    if (FormattedNames.TryGetValue(format, out string[] formattedNames))
                    {
                        return formattedNames;
                    }

                    throw new ArgumentOutOfRangeException($"Unknown format: {format}. Known formats are: null, \"\", 'G', 'g', 'F', 'f', 'D', 'd', 'X' or 'x'.");
            }
        }

        /// <summary>
        /// Retrieve the string representation for the provided enum value. The enum alias support object determines what attributes are supported
        /// in aliasing enum values. The provided format determines how the enum value should be converted to a string, if no alias is defined.
        /// </summary>
        /// <param name="value">Teh enum value to get the string representation for.</param>
        /// <param name="enumAliasSupport">The enum alias feature that determines the attributes used to get an alias for the enum value.</param>
        /// <param name="format">The string format to convert the enum value if no alias was found. Supported formats are: G, g, F, f, D, d, X and x.</param>
        /// <returns>A string representation of the enum value, possibly aliased by the enum alias feature.</returns>
        public string GetStringRepresentationFor(Enum value, IEnumAliasFeature enumAliasSupport, string format)
        {
            enumAliasSupport.ThrowIfNull(nameof(enumAliasSupport));
            enumAliasSupport.AliasValueAttribute.ThrowIfNull(nameof(enumAliasSupport.AliasValueAttribute));

            // Check first that the enum defines an alias. If so
            CachedEnumEntry[] aliasEntries = FindEnumAliases(enumAliasSupport.AliasValueAttribute);
            if (Values.TryFindIndex((v => v.Equals(value)), out int resultIndex))
            {
                return aliasEntries[resultIndex].StringBasedRepresentation; // This works because the alias entries are in the same sequence as the name and value arrays.
            }

            if (!IsFlag)
            {
                return value.ToString(format);
            }

            // Explode the current result.
            string[] enumStringValues = value.ToString(format).Split(EnumFlagSplit, StringSplitOptions.RemoveEmptyEntries);

            // Check each value name whether an alias is available.
            for (int i = 0; i < enumStringValues.Length; ++i)
            {
                if (GetNamesForFormat(format).TryFindIndex(n => n.Equals(enumStringValues[i]), out resultIndex))
                {
                    enumStringValues[i] = aliasEntries[resultIndex].StringBasedRepresentation;
                }
            }

            // Stitch back together the result based on the replaced values.
            return string.Join(EnumFlagSplit, enumStringValues);
        }

        /// <summary>
        /// Find the enum value for the provided string value. The string value is first matched to the set of aliases defined by the enum alias feature.
        /// If no alias could be matched, then one of the original names of the Enum is searched for.
        /// If no original name could be matched, then the value is parsed if the enum doesn't define a Flags attribute, or isn't a composite enum value.
        /// If the value represents a composite enum value, then each of the values is tried and matched for an alias, before being attempted to be parsed again. 
        /// </summary>
        /// <param name="value">The string value to be converted to an enum value.</param>
        /// <param name="enumAliasSupport">The enum alias feature that determines the attributes used to get an alias for the enum value.</param>
        /// <returns>The enum value representation of the provided string value.</returns>
        public Enum GetEnumValueFor(string value, IEnumAliasFeature enumAliasSupport)
        {
            enumAliasSupport.ThrowIfNull(nameof(enumAliasSupport));
            enumAliasSupport.AliasValueAttribute.ThrowIfNull(nameof(enumAliasSupport.AliasValueAttribute));

            CachedEnumEntry[] aliasEntries = FindEnumAliases(enumAliasSupport.AliasValueAttribute);
            if (aliasEntries.TryFindIndex((ae => ae.alias.Equals(value)), out int resultIndex))
            {
                return aliasEntries[resultIndex].value;
            }

            if (Names.TryFindIndex((n => n.Equals(value)), out resultIndex))
            {
                return Values[resultIndex];
            }

            // If the enum does not contain flags, then just try and parse the string value.
            if (!IsFlag || !value.Contains(EnumFlagSplit))
            {
                return Enum.Parse(Type, value) as Enum;
            }

            // Replace aliases with their original string counterparts so that it can be properly parsed.
            string[] splitValues = value.Split(EnumFlagSplit, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < splitValues.Length; ++i)
            {
                if (aliasEntries.TryFindIndex((ae => ae.alias.Equals(splitValues[i])), out resultIndex))
                {
                    splitValues[i] = aliasEntries[resultIndex].name;
                }
            }

            return Enum.Parse(Type, string.Join(EnumFlagSplit, splitValues)) as Enum;
        }

        private Attribute[] FindTypeDefinedAttributes(Type attributeType)
        {
            if (typeDefinedAttributes.TryGetValue(attributeType, out Attribute[] r))
            {
                return r;
            }

            Attribute[] attributes = Attribute.GetCustomAttributes(Type, attributeType, false); // Enums can't be inherited from, so no need to search further.
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
                FieldInfo field = Type.GetField(name); // Field can be retrieved based on the name of the value.
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

            public string StringBasedRepresentation => !string.IsNullOrEmpty(alias) ? alias : name;
        }
    }
}