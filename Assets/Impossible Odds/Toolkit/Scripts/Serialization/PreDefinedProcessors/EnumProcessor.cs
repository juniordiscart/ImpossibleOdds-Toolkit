namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	/// <summary>
	/// A (de)serialization processor for enum values. This includes potential alias values for the enum values.
	/// </summary>
	public class EnumProcessor : ISerializationProcessor, IDeserializationProcessor
	{
		private static readonly string[] EnumFlagSplit = new string[] { ", " };
		private static Dictionary<Type, EnumTypeCache> enumTypeCache = new Dictionary<Type, EnumTypeCache>();

		private ISerializationDefinition definition = null;
		private IEnumAliasSupport enumAliasSupport = null;

		public bool SupportsEnumAlias
		{
			get { return enumAliasSupport != null; }
		}

		public ISerializationDefinition Definition
		{
			get { return definition; }
		}

		public EnumProcessor(ISerializationDefinition definition)
		{
			this.definition = definition;
			this.enumAliasSupport = (definition is IEnumAliasSupport) ? (definition as IEnumAliasSupport) : null;
		}

		/// <summary>
		/// Attempts to serialize the enum value to a supported type defined by the serialization definition. If an alias is defined, then the alias is serialized.
		/// </summary>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <param name="serializedResult">The serialized object.</param>
		/// <returns>True if the serialization is compatible and accepted, false otherwise.</returns>
		public bool Serialize(object objectToSerialize, out object serializedResult)
		{
			Type sourceType = objectToSerialize.GetType();
			if ((objectToSerialize == null) || !sourceType.IsEnum)
			{
				serializedResult = null;
				return false;
			}

			EnumTypeCache enumTypeCache = GetEnumTypeCache(sourceType);

			// If the definition supports enum aliases and is preferred to be sent serialized as string.
			if (SupportsEnumAlias && enumTypeCache.PrefersStringBasedRepresentation(enumAliasSupport.EnumAsStringAttributeType))
			{
				// Check the enum value. If it isn't found in the original set,
				// then it's a composite value and aliases may need to be inserted in the final result.
				if (enumTypeCache.IsValueDefined(objectToSerialize))
				{
					string enumStringValue =
						enumTypeCache.IsValueAliased(objectToSerialize, enumAliasSupport.EnumAliasValueAttributeType) ?
						enumTypeCache.GetAlias(objectToSerialize, enumAliasSupport.EnumAliasValueAttributeType) :
						((Enum)objectToSerialize).ToString();

					serializedResult = Serializer.Serialize(enumStringValue, Definition);
				}
				else if (enumTypeCache.IsFlags)
				{
					// Explode the current result.
					IList<string> enumStringValues = ((Enum)objectToSerialize).ToString().Split(EnumFlagSplit, StringSplitOptions.RemoveEmptyEntries);

					// Check each value name whether an alias is available.
					for (int i = 0; i < enumStringValues.Count; ++i)
					{
						string enumStringValue = enumStringValues[i];
						FieldInfo enumField = enumTypeCache.NameToFieldMapping[enumStringValue];
						if (enumTypeCache.IsValueAliased(enumField, enumAliasSupport.EnumAliasValueAttributeType))
						{
							enumStringValues[i] = enumTypeCache.GetAlias(enumField, enumAliasSupport.EnumAliasValueAttributeType);
						}
					}

					serializedResult = string.Join(EnumFlagSplit[0], enumStringValues);
				}
				else
				{
					// throw new SerializationException("The value of '{0}' associated with enum type {1} is a value that cannot be represented by a string.", objectToSerialize.ToString(), sourceType.Name);
					// throw new SerializationException("Enum value {0} of type {1} is a non-flagged composite value.", objectToSerialize.ToString(), sourceType.Name);

					// Just serialize the value as a string.
					serializedResult = objectToSerialize.ToString();
				}

				return true;
			}

			// By this point, the enum should be serialized using its underlying type.
			serializedResult = Serializer.Serialize(Convert.ChangeType(objectToSerialize, Enum.GetUnderlyingType(sourceType)), Definition);
			return true;
		}

		/// <summary>
		/// Attempts to deserialize the data to an enum value of the target type.
		/// </summary>
		/// <param name="targetType">The target type to deserialize the given data.</param>
		/// <param name="dataToDeserialize">The data deserialize and apply to the result.</param>
		/// <param name="deserializedResult">The result unto which the data is applied.</param>
		/// <returns>True if deserialization is compatible and accepted, false otherwise.</returns>
		public bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult)
		{
			// If we're not dealing with an enum target, then don't bother.
			if (!targetType.IsEnum || (dataToDeserialize == null))
			{
				deserializedResult = null;
				return false;
			}

			// Check if the value is a string.
			if (dataToDeserialize is string dataStr)
			{
				// If the string represents a numeric value, try to match to an internal value of the enum.
				// This may throw an exception if the value is not a numeric one. We let that fail silently and try a different approach.
				try
				{
					deserializedResult = Enum.ToObject(targetType, Convert.ChangeType(dataStr, Enum.GetUnderlyingType(targetType)));
					return true;
				}
				catch (FormatException)
				{ }

				// If aliasing is supported, try to process individual values in the enum string
				if (SupportsEnumAlias)
				{
					EnumTypeCache enumTypeCache = GetEnumTypeCache(targetType);
					IList<string> enumStrValues = dataStr.Split(EnumFlagSplit, StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < enumStrValues.Count; ++i)
					{
						string enumStrValue = enumStrValues[i];

						// If the name isn't defined, check whether it is an alias for one of the fields
						if (!enumTypeCache.IsNameDefined(enumStrValue))
						{
							FieldInfo aliasedField = enumTypeCache.GetFieldForAlias(enumStrValue, enumAliasSupport.EnumAliasValueAttributeType);
							if (aliasedField != null)
							{
								enumStrValues[i] = aliasedField.Name;
							}
						}
					}

					dataStr = string.Join(EnumFlagSplit[0], enumStrValues);
				}

				deserializedResult = Enum.Parse(targetType, dataStr);
				return true;
			}
			else if (dataToDeserialize.GetType().IsValueType)
			{
				Type underlyingEnumType = Enum.GetUnderlyingType(targetType);
				deserializedResult = Enum.ToObject(targetType, Convert.ChangeType(dataToDeserialize, underlyingEnumType));
				return true;
			}
			else
			{
				throw new SerializationException("The provided data to deserialize to enum of type {0} is not a {1} or a value type.", dataToDeserialize.GetType().Name, typeof(string).Name);
			}
		}

		private EnumTypeCache GetEnumTypeCache(Type enumType)
		{
			enumType.ThrowIfNull(nameof(enumType));

			if (!enumType.IsEnum)
			{
				throw new ArgumentException(string.Format("The type {0} is not an enum.", enumType.Name));
			}

			if (!enumTypeCache.ContainsKey(enumType))
			{
				enumTypeCache[enumType] = new EnumTypeCache(enumType);
			}

			return enumTypeCache[enumType];
		}

		/// <summary>
		/// Binds an alias attribute to a field for quick access.
		/// </summary>
		private struct FieldAtrributeTuple
		{
			public FieldInfo Field
			{
				get; private set;
			}

			public IEnumAliasParameter AliasAttribute
			{
				get; private set;
			}

			public FieldAtrributeTuple(FieldInfo field, IEnumAliasParameter aliasAttribute)
			{
				Field = field;
				AliasAttribute = aliasAttribute;
			}
		}

		/// <summary>
		/// Lazy type cache for commonly used data in processing enum aliases.
		/// </summary>
		private class EnumTypeCache
		{
			private readonly Type enumType;
			private readonly bool isFlagged;
			private readonly List<FieldInfo> enumFields;
			private readonly Dictionary<object, FieldInfo> valueToFieldMapping;
			private readonly Dictionary<string, FieldInfo> nameToFieldMapping;
			private Dictionary<Type, bool> preferStringBasedRepresentation = new Dictionary<Type, bool>();
			private Dictionary<Type, List<FieldAtrributeTuple>> aliasedFields = new Dictionary<Type, List<FieldAtrributeTuple>>();

			/// <summary>
			/// Type of the enum.
			/// </summary>
			public Type Type
			{
				get { return enumType; }
			}

			/// <summary>
			/// Is the enum flagged with the System.Flags attribute?
			/// </summary>
			public bool IsFlags
			{
				get { return isFlagged; }
			}

			/// <summary>
			/// The backing fields of the values of the enum.
			/// </summary>
			public IReadOnlyList<FieldInfo> EnumFields
			{
				get { return enumFields; }
			}

			/// <summary>
			/// Mapping of the enum values to their backing field.
			/// </summary>
			public IReadOnlyDictionary<object, FieldInfo> ValueToFieldMapping
			{
				get { return valueToFieldMapping; }
			}

			/// <summary>
			/// Mapping of the enum names to their backing field.
			/// </summary>
			public IReadOnlyDictionary<string, FieldInfo> NameToFieldMapping
			{
				get { return nameToFieldMapping; }
			}

			public EnumTypeCache(Type enumType)
			{
				enumType.ThrowIfNull(nameof(enumType));

				if (!enumType.IsEnum)
				{
					throw new ArgumentException("The type {0} is not an enum.", enumType.Name);
				}

				this.enumType = enumType;
				this.isFlagged = enumType.IsDefined(typeof(FlagsAttribute), false);

				// Create mappings to the fields for quick access.
				IList enumValues = Enum.GetValues(enumType);
				enumFields = new List<FieldInfo>(enumValues.Count);
				valueToFieldMapping = new Dictionary<object, FieldInfo>(enumValues.Count);
				nameToFieldMapping = new Dictionary<string, FieldInfo>(enumValues.Count);
				foreach (object enumValue in enumValues)
				{
					FieldInfo field = enumType.GetField(enumValue.ToString());
					enumFields.Add(field);
					valueToFieldMapping.Add(enumValue, field);
					nameToFieldMapping.Add(field.Name, field);
				}
			}

			/// <summary>
			/// Check whether the enum prefers to be represented as a string by evaluating whether the attribute is defined on it.
			/// </summary>
			/// <param name="typeOfAttribute">Type of the attribute to check for.</param>
			/// <returns>True if the attribute is present, and thus prefers a string-based representation. False otherwise.</returns>
			public bool PrefersStringBasedRepresentation(Type typeOfAttribute)
			{
				typeOfAttribute.ThrowIfNull(nameof(typeOfAttribute));

				if (!preferStringBasedRepresentation.ContainsKey(typeOfAttribute))
				{
					preferStringBasedRepresentation[typeOfAttribute] = enumType.IsDefined(typeOfAttribute, false);
				}

				return preferStringBasedRepresentation[typeOfAttribute];
			}

			/// <summary>
			/// Checks whether the given value is explicitly defined on the enum.
			/// </summary>
			/// <param name="value">The value to check for.</param>
			/// <returns>True if the value is explicitly defined. False otherwise.</returns>
			public bool IsValueDefined(object value)
			{
				value.ThrowIfNull(nameof(value));
				return valueToFieldMapping.ContainsKey(value);
			}

			/// <summary>
			/// Checks whether the given name is explicitly defined on the enum.
			/// </summary>
			/// <param name="name">The name to check for.</param>
			/// <returns>True if the name is explicitly defined. False otherwise.</returns>
			public bool IsNameDefined(string name)
			{
				name.ThrowIfNullOrWhitespace(name);
				return nameToFieldMapping.ContainsKey(name);
			}

			/// <summary>
			/// Retrieves the collection of aliased fields with the given type of attribute.
			/// </summary>
			/// <param name="typeOfAttribute">The type of the attribute to evaluate the enum's field against.</param>
			/// <returns>The collection of aliased fields of the enum.</returns>
			public IReadOnlyList<FieldAtrributeTuple> GetAliasedFields(Type typeOfAttribute)
			{
				typeOfAttribute.ThrowIfNull(nameof(typeOfAttribute));

				if (aliasedFields.ContainsKey(typeOfAttribute))
				{
					return aliasedFields[typeOfAttribute];
				}

				if (!typeof(IEnumAliasParameter).IsAssignableFrom(typeOfAttribute))
				{
					throw new ArgumentException(string.Format("The type {0} does not implement the {1} interface.", typeOfAttribute.Name, typeof(IEnumAliasParameter).Name));
				}

				// Get all fields on which the attribute is defined.
				List<FieldAtrributeTuple> aliasedFieldTuples = new List<FieldAtrributeTuple>(enumFields.Count);
				foreach (FieldInfo enumField in enumFields)
				{
					if (enumField.IsDefined(typeOfAttribute, false))
					{
						IEnumAliasParameter alias = enumField.GetCustomAttribute(typeOfAttribute, false) as IEnumAliasParameter;
						aliasedFieldTuples.Add(new FieldAtrributeTuple(enumField, alias));
					}
				}

				aliasedFields[typeOfAttribute] = aliasedFieldTuples;
				return aliasedFields[typeOfAttribute];
			}

			/// <summary>
			/// Checks whether the field of the enum has an alias defined for the given attribute type.
			/// </summary>
			/// <param name="field">The enum field to check.</param>
			/// <param name="typeOfAttribute">The attribute type to evaluate against.</param>
			/// <returns>True if an alias is defined for this field, false otherwise.</returns>
			public bool IsValueAliased(FieldInfo field, Type typeOfAttribute)
			{
				field.ThrowIfNull(nameof(field));
				typeOfAttribute.ThrowIfNull(nameof(typeOfAttribute));

				if (!enumFields.Contains(field))
				{
					throw new ArgumentException(string.Format("The field {0} is not a registered field with enum {1}.", field.Name, enumType.Name));
				}

				IReadOnlyList<FieldAtrributeTuple> aliasedFields = GetAliasedFields(typeOfAttribute);
				return aliasedFields.Any(aF => aF.Field == field);
			}

			/// <summary>
			/// Checks whether the value of the enum is aliased for the given attribute type.
			/// </summary>
			/// <param name="value">The value of the enum to check.</param>
			/// <param name="typeOfAttribute">The attribute type to evaluate against.</param>
			/// <returns>True if an alias is defined for this value. False otherwise.</returns>
			public bool IsValueAliased(object value, Type typeOfAttribute)
			{
				value.ThrowIfNull(nameof(value));
				typeOfAttribute.ThrowIfNull(nameof(typeOfAttribute));

				if (!IsValueDefined(value))
				{
					throw new SerializationException("The value {0} is not a value defined on the enum of type {1}.", value.ToString(), enumType.Name);
				}

				return IsValueAliased(valueToFieldMapping[value], typeOfAttribute);
			}

			/// <summary>
			/// Retrieve the alias of an enum field with a given attribute type.
			/// </summary>
			/// <param name="field">The enum field to retrieve the alias for.</param>
			/// <param name="typeOfAttribute">The attribute type to evaluate against.</param>
			/// <returns>The defined alias for the enum field.</returns>
			public string GetAlias(FieldInfo field, Type typeOfAttribute)
			{
				field.ThrowIfNull(nameof(field));
				typeOfAttribute.ThrowIfNull(nameof(typeOfAttribute));

				if (!IsValueAliased(field, typeOfAttribute))
				{
					throw new SerializationException("The requested field {0} on enum {1} has no alias defined for attribute of type.", field.Name, enumType.Name, typeOfAttribute.Name);
				}

				IReadOnlyList<FieldAtrributeTuple> aliasedFields = GetAliasedFields(typeOfAttribute);
				return aliasedFields.Single(aF => aF.Field == field).AliasAttribute.Alias;
			}

			/// <summary>
			/// Retrieve the alias of an enum value with a given attribute type.
			/// </summary>
			/// <param name="value">The enum value to retrieve the alias for.</param>
			/// <param name="typeOfAttribute">The attribute type to evaluate against.</param>
			/// <returns>The defined alias for the enum value.</returns>
			public string GetAlias(object value, Type typeOfAttribute)
			{
				value.ThrowIfNull(nameof(value));
				typeOfAttribute.ThrowIfNull(nameof(typeOfAttribute));

				if (!IsValueDefined(value))
				{
					throw new SerializationException("The value {0} is not a value defined on the enum of type {1}.", value.ToString(), enumType.Name);
				}

				return GetAlias(valueToFieldMapping[value], typeOfAttribute);
			}

			public FieldInfo GetFieldForAlias(string alias, Type typeOfAttribute)
			{
				alias.ThrowIfNullOrWhitespace(nameof(alias));
				typeOfAttribute.ThrowIfNull(nameof(typeOfAttribute));

				IReadOnlyList<FieldAtrributeTuple> aliasedFields = GetAliasedFields(typeOfAttribute);
				return aliasedFields.Any(aF => string.Equals(aF.AliasAttribute.Alias, alias)) ? aliasedFields.First(aF => string.Equals(aF.AliasAttribute.Alias, alias)).Field : null;
			}
		}
	}
}
