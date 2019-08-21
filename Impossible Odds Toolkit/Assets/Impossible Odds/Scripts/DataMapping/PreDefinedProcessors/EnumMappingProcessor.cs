namespace ImpossibleOdds.DataMapping.Processors
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Reflection;

	/// <summary>
	/// Mapping processor for enum values. This includes potential alias values for the enum values.
	/// </summary>
	public class EnumMappingProcessor : AbstractMappingProcessor, IMapToDataStructureProcessor, IMapFromDataStructureProcessor
	{
		private static Dictionary<Type, List<FieldInfo>> aliasAttributesCache = new Dictionary<Type, List<FieldInfo>>();

		public EnumMappingProcessor(IMappingDefinition definition)
		: base(definition)
		{ }

		/// <summary>
		/// Attempts to map the enum value to a supported type defined by the mapping definition. If an alias is defined, then the alias is processed.
		/// </summary>
		/// <param name="sourceValue">The source value to process.</param>
		/// <param name="objResult">The result in which the value will be stored.</param>
		/// <returns>True if the mapping is accepted for processing, false otherwise.</returns>
		public bool MapToDataStructure(object sourceValue, out object objResult)
		{
			if ((sourceValue == null) || !sourceValue.GetType().IsEnum)
			{
				objResult = null;
				return false;
			}

			Type sourceType = sourceValue.GetType();

			// Check if an encoding alias for the enum is defined
			// We have to check for a null, since an enum can be a combo of different
			// values and then no field info will exist.
			FieldInfo enumField = sourceType.GetField(sourceValue.ToString());
			if (enumField != null)
			{
				object aliasAttr = enumField.GetCustomAttributes(typeof(EnumStringAliasAttribute), false).SingleOrDefault();
				if (aliasAttr != null)
				{
					string aliasValue = (aliasAttr as EnumStringAliasAttribute).aliasValue;
					if (!definition.SupportedProcessingTypes.Contains(aliasValue.GetType()))
					{
						throw new DataMappingException("An enum value has specifically specified an alias value, but the value's type isn't a supported type.");
					}

					if (!definition.SupportedProcessingTypes.Contains(aliasValue.GetType()))
					{
						throw new DataMappingException("The serialized value of the enum is not supported.");
					}

					objResult = aliasValue;
					return true;
				}
			}

			// Check whether the enum is requested to be sent as a string, or its underlying value
			object enumValue = null;
			if (sourceType.IsDefined(typeof(EnumStringMappingAttribute), false))
			{
				enumValue = ((Enum)sourceValue).ToString();
			}
			else
			{
				enumValue = Convert.ChangeType(sourceValue, Enum.GetUnderlyingType(sourceType));
			}

			if (!definition.SupportedProcessingTypes.Contains(enumValue.GetType()))
			{
				throw new DataMappingException("The serialized value of the enum is not supported.");
			}

			objResult = enumValue;
			return true;
		}

		/// <summary>
		/// Attempts to map the object to process to a value enum value.
		/// </summary>
		/// <param name="targetType">The target type the caller expects the result to be.</param>
		/// <param name="objToProcess">The object to process to a valid enum value.</param>
		/// <param name="objResult">The result in which the value will be stored.</param>
		/// <returns>True if the mapping is accepted for processing, false otherwise.</returns>
		public bool MapFromDataStructure(Type targetType, object objToProcess, out object objResult)
		{
			// If we're not dealing with an enum target, then don't bother
			if (!targetType.IsEnum || (objToProcess == null))
			{
				objResult = null;
				return false;
			}

			// Check if the value is a string
			if (typeof(string).IsAssignableFrom(objToProcess.GetType()))
			{
				// Get the enum fields that have potential aliases defined
				ReadOnlyCollection<FieldInfo> aliasFields = GetEnumAliases(targetType);
				foreach (FieldInfo aliasField in aliasFields)
				{
					EnumStringAliasAttribute aliasAttr = aliasField.GetCustomAttributes(typeof(EnumStringAliasAttribute), false).First() as EnumStringAliasAttribute;

					// If we found an alias that matches the given string value
					if (aliasAttr.aliasValue.Equals(objToProcess))
					{
						objResult = aliasField.GetValue(null);
						return true;
					}
				}

				// Last resort
				objResult = Enum.Parse(targetType, objToProcess as string, true);
			}
			else
			{
				objResult = Enum.ToObject(targetType, objToProcess);
			}

			return true;
		}

		private ReadOnlyCollection<FieldInfo> GetEnumAliases(Type enumType)
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
