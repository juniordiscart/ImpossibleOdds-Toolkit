namespace ImpossibleOdds.DataMapping.Processors
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Mapping processor for primitive types, i.e. int, bool, float, ...
	/// </summary>
	public class PrimitiveTypeMappingProcessor : AbstractMappingProcessor, IMapToDataStructureProcessor, IMapFromDataStructureProcessor
	{
		private const string TRUE_STRING_ALT = "1";
		private const string FALSE_STRING_ALT = "0";

		private static List<Type> primitiveTypeOrder = new List<Type>()
		{
			typeof(bool),
			typeof(byte),
			typeof(sbyte),
			typeof(char),
			typeof(ushort),
			typeof(short),
			typeof(uint),
			typeof(int),
			typeof(ulong),
			typeof(long),
			typeof(float),
			typeof(double)
		};

		public PrimitiveTypeMappingProcessor(IMappingDefinition definition)
		: base(definition)
		{ }

		/// <summary>
		/// Attempt to map the source value to a single instance of a primitive type as defined by the mapping definition.
		/// </summary>
		/// <param name="sourceValue">The value to map to a primitive type.</param>
		/// <param name="objResult">The result in which the mapped value is stored.</param>
		/// <returns>True if the mapping is accepted for processing, false otherwise.</returns>
		public bool MapToDataStructure(object sourceValue, out object objResult)
		{
			if ((sourceValue == null) || !sourceValue.GetType().IsPrimitive)
			{
				objResult = null;
				return false;
			}

			Type sourceType = sourceValue.GetType();

			// Check whether the type is supported after all...
			if (definition.SupportedProcessingTypes.Contains(sourceType))
			{
				objResult = sourceValue;
				return true;
			}

			// Attempt to convert the source value to a supported primitive type that is higher in the chain.
			if (primitiveTypeOrder.Contains(sourceType))
			{
				for (int i = primitiveTypeOrder.IndexOf(sourceType); i < primitiveTypeOrder.Count; ++i)
				{
					Type targetType = primitiveTypeOrder[i];
					if (!definition.SupportedProcessingTypes.Contains(targetType))
					{
						continue;
					}

					objResult = Convert.ChangeType(sourceValue, targetType);
					return true;
				}
			}

			// TODO: check whether an implicit or explicit type conversion exists that may have been defined by the user.

			// Last resort: check whether a string-type is accepted.
			if (definition.SupportedProcessingTypes.Contains(typeof(string)))
			{
				objResult = sourceValue.ToString();
				return true;
			}

			throw new DataMappingException(string.Format("Cannot convert the primitive data type {0} to a data type that is supported by the mapping definition of type {1}.", sourceType.Name, definition.GetType().Name));
		}

		/// <summary>
		/// Attempts to map the object to process to an instance of a primitive type.
		/// </summary>
		/// <param name="targetType">The expected type of the result to be returned.</param>
		/// <param name="objToProcess">The object to process to a primitive type.</param>
		/// <param name="objResult">The result in which the value will be stored.</param>
		/// <returns>True if the mapping is accepted for processing, false otherwise.</returns>
		public bool MapFromDataStructure(Type targetType, object objToProcess, out object objResult)
		{
			if ((targetType == null) || !targetType.IsPrimitive || (objToProcess == null))
			{
				objResult = null;
				return false;
			}

			// For booleans, we do an additional test.
			if (typeof(bool) == targetType)
			{
				objResult = ConvertToBoolean(objToProcess);
			}
			else
			{
				objResult = Convert.ChangeType(objToProcess, targetType);
			}
			return true;
		}

		private object ConvertToBoolean(object objToParse)
		{
			if (typeof(string).IsAssignableFrom(objToParse.GetType()))
			{
				string strValue = (string)objToParse;
				switch (strValue)
				{
					case TRUE_STRING_ALT:
						return true;
					case FALSE_STRING_ALT:
						return false;
					default:
						return Convert.ChangeType(objToParse, typeof(bool));
				}
			}

			return Convert.ChangeType(objToParse, typeof(bool));
		}
	}
}
