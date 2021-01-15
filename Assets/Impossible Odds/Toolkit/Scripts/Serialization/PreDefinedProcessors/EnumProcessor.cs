namespace ImpossibleOdds.Serialization.Processors
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	/// <summary>
	/// A (de)serialization processor for enum values. This includes potential alias values for the enum values.
	/// </summary>
	public class EnumProcessor : ISerializationProcessor, IDeserializationProcessor
	{
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

			// If the definition supports enum aliases and is preferred to be sent serialized as string.
			if (SupportsEnumAlias && IsEnumStringPreferred(enumAliasSupport, sourceType))
			{
				FieldInfo enumField = sourceType.GetField(objectToSerialize.ToString());
				string enumStringValue =
					IsEnumValueAliased(enumAliasSupport, enumField) ?
					GetEnumAliasValue(enumAliasSupport, enumField) :
					((Enum)objectToSerialize).ToString();

				serializedResult = Serializer.Serialize(enumStringValue, Definition);
				return true;
			}

			// By this point, the enum should be serialized under its underlying type.
			object enumValue = Convert.ChangeType(objectToSerialize, Enum.GetUnderlyingType(sourceType));
			serializedResult = Serializer.Serialize(enumValue, Definition);
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
			if (typeof(string).IsAssignableFrom(dataToDeserialize.GetType()))
			{
				string dataStr = dataToDeserialize as string;

				// Get the named values of the enum, and see if the incoming data can be matched.
				// If so, then the actual enum value can be retrieved already.
				string[] enumNames = Enum.GetNames(targetType);
				if (enumNames.Contains(dataStr))
				{
					deserializedResult = Enum.Parse(targetType, dataStr);
					return true;
				}

				// Find the aliased field.
				IReadOnlyList<FieldInfo> aliasFields = GetAliasedFields(enumAliasSupport, targetType);
				FieldInfo aliasField = aliasFields.FirstOrDefault(af => GetEnumAliasValue(enumAliasSupport, af).Equals(dataStr));
				if (aliasField != null)
				{
					deserializedResult = aliasField.GetValue(null);
					return true;
				}

				// If no
			}

			deserializedResult = Enum.ToObject(targetType, dataToDeserialize);
			return true;
		}

		/// <summary>
		/// Checks whether the enum type is flagged with the System.Flags attribute.
		/// </summary>
		/// <param name="enumType">Type of the enum to check.</param>
		/// <returns>True if the Flags attribute is set.</returns>
		private bool IsEnumFlagged(Type enumType)
		{
			enumType.ThrowIfNull(nameof(enumType));
			if (!enumType.IsEnum)
			{
				throw new SerializationException("The type {0} is not an enum.", enumType.Name);
			}

			return enumType.IsDefined(typeof(FlagsAttribute), false);
		}

		/// <summary>
		/// Checks whether the enum type is preferred to be processed as a string value rather than its internal value.
		/// </summary>
		/// <param name="definition">Definition that provides the attribute to check for on the enum type.</param>
		/// <param name="enumType">The enum type to check if it prefers to be processed as a string.</param>
		/// <returns>True if the enum type prefers to be processed as a string value.</returns>
		private bool IsEnumStringPreferred(IEnumAliasSupport definition, Type enumType)
		{
			definition.ThrowIfNull(nameof(definition));
			enumType.ThrowIfNull(nameof(enumType));

			if (!enumType.IsEnum)
			{
				throw new SerializationException("The type {0} is not an enum.", enumType.Name);
			}

			return enumType.IsDefined(definition.EnumAsStringAttributeType, false);
		}

		/// <summary>
		/// Checks whether the field of the enum has an alias defined.
		/// </summary>
		/// <param name="definition">Definition that provides the attribute to check for on the enum field.</param>
		/// <param name="enumField">The field to check whether it has an alias defined.</param>
		/// <returns>True if an alias is defined.</returns>
		private bool IsEnumValueAliased(IEnumAliasSupport definition, FieldInfo enumField)
		{
			definition.ThrowIfNull(nameof(definition));
			enumField.ThrowIfNull(nameof(enumField));

			return enumField.IsDefined(definition.EnumAliasValueAttributeType, false);
		}

		/// <summary>
		/// Get the alias defined for the enum field.
		/// </summary>
		/// <param name="definition">Definition that provides the attribute to retrieve the alias of the enum value.</param>
		/// <param name="enumField">The field to retrieve the alias value for.</param>
		/// <returns>The alias value for the enum value.</returns>
		private string GetEnumAliasValue(IEnumAliasSupport definition, FieldInfo enumField)
		{
			definition.ThrowIfNull(nameof(definition));
			enumField.ThrowIfNull(nameof(enumField));

			IEnumAliasParameter aliasAttr = enumField.GetCustomAttribute(definition.EnumAliasValueAttributeType) as IEnumAliasParameter;
			return (aliasAttr != null) ? aliasAttr.Alias : string.Empty;
		}

		/// <summary>
		/// Get all fields on the enum that have an alias defined.
		/// </summary>
		/// <param name="definition">Definition that provides the attribute to retrieve the aliases from the enum values.</param>
		/// <param name="enumType">The enum type to retrieve the alias fields for.</param>
		/// <returns>A list of fields with an alias defined.</returns>
		private IReadOnlyList<FieldInfo> GetAliasedFields(IEnumAliasSupport definition, Type enumType)
		{
			definition.ThrowIfNull(nameof(definition));
			enumType.ThrowIfNull(nameof(enumType));

			if (!enumType.IsEnum)
			{
				throw new SerializationException("The type {0} is not an enum.", enumType.Name);
			}

			return enumType.GetFields().Where(enumField => enumField.IsDefined(enumAliasSupport.EnumAsStringAttributeType, false)).ToList();
		}
	}
}
