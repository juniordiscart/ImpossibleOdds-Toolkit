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
		private static Dictionary<Type, List<FieldInfo>> aliasAttributesCache = new Dictionary<Type, List<FieldInfo>>();

		public ISerializationDefinition Definition
		{
			get { return definition; }
		}


		public EnumProcessor(ISerializationDefinition definition)
		{
			this.definition = definition;
		}

		/// <summary>
		/// Attempts to serialize the enum value to a supported type defined by the serialization definition. If an alias is defined, then the alias is serialized.
		/// </summary>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <param name="serializedResult">The serialized object.</param>
		/// <returns>True if the serialization is compatible and accepted, false otherwise.</returns>
		public bool Serialize(object objectToSerialize, out object serializedResult)
		{
			if ((objectToSerialize == null) || !objectToSerialize.GetType().IsEnum)
			{
				serializedResult = null;
				return false;
			}

			Type sourceType = objectToSerialize.GetType();

			// Check if an encoding alias for the enum is defined
			// We have to check for a null, since an enum can be a combo of different
			// values and then no field info will exist.
			FieldInfo enumField = sourceType.GetField(objectToSerialize.ToString());
			if (enumField != null)
			{
				object aliasAttr = enumField.GetCustomAttributes(typeof(EnumStringAliasAttribute), false).SingleOrDefault();
				if (aliasAttr != null)
				{
					string aliasValue = (aliasAttr as EnumStringAliasAttribute).aliasValue;
					if (!definition.SupportedTypes.Contains(aliasValue.GetType()))
					{
						throw new SerializationException("An enum value has specifically specified an alias value, but the value's type isn't a supported type.");
					}

					if (!definition.SupportedTypes.Contains(aliasValue.GetType()))
					{
						throw new SerializationException("The serialized value of the enum is not supported.");
					}

					serializedResult = aliasValue;
					return true;
				}
			}

			// Check whether the enum is requested to be sent as a string, or its underlying value
			object enumValue = null;
			if (sourceType.IsDefined(typeof(EnumStringSerializationAttribute), false))
			{
				enumValue = ((Enum)objectToSerialize).ToString();
			}
			else
			{
				enumValue = Convert.ChangeType(objectToSerialize, Enum.GetUnderlyingType(sourceType));
			}

			if (!definition.SupportedTypes.Contains(enumValue.GetType()))
			{
				throw new SerializationException("The serialized value of the enum is not supported.");
			}

			serializedResult = enumValue;
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
			// If we're not dealing with an enum target, then don't bother
			if (!targetType.IsEnum || (dataToDeserialize == null))
			{
				deserializedResult = null;
				return false;
			}

			// Check if the value is a string
			if (typeof(string).IsAssignableFrom(dataToDeserialize.GetType()))
			{
				// Get the enum fields that have potential aliases defined
				IReadOnlyList<FieldInfo> aliasFields = GetEnumAliases(targetType);
				foreach (FieldInfo aliasField in aliasFields)
				{
					EnumStringAliasAttribute aliasAttr = aliasField.GetCustomAttributes(typeof(EnumStringAliasAttribute), false).First() as EnumStringAliasAttribute;

					// If we found an alias that matches the given string value
					if (aliasAttr.aliasValue.Equals(dataToDeserialize))
					{
						deserializedResult = aliasField.GetValue(null);
						return true;
					}
				}

				// Last resort
				deserializedResult = Enum.Parse(targetType, dataToDeserialize as string, true);
			}
			else
			{
				deserializedResult = Enum.ToObject(targetType, dataToDeserialize);
			}

			return true;
		}

		private IReadOnlyList<FieldInfo> GetEnumAliases(Type enumType)
		{
			if (aliasAttributesCache.ContainsKey(enumType))
			{
				return aliasAttributesCache[enumType].AsReadOnly();
			}

			List<FieldInfo> aliasFields = enumType.GetFields().Where(f => f.IsDefined(typeof(EnumStringAliasAttribute), false)).ToList();
			aliasAttributesCache.Add(enumType, aliasFields);
			return aliasFields.AsReadOnly();
		}
	}
}
