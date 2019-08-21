namespace ImpossibleOdds.DataMapping.Processors
{
	using System;

	/// <summary>
	/// Mapping processor specifically for the Decimal type.
	/// </summary>
	public class DecimalMappingProcessor : AbstractMappingProcessor, IMapToDataStructureProcessor, IMapFromDataStructureProcessor
	{
		public DecimalMappingProcessor(IMappingDefinition definition)
		: base(definition)
		{ }

		/// <summary>
		/// Attempts to map the source value as a Decimal to a supported type as defined by the mapping definition.
		/// </summary>
		/// <param name="sourceValue">The source value to process as a Decimal type.</param>
		/// <param name="objResult">The result in which the value will be stored.</param>
		/// <returns>True if the mapping is accepted for processing, false otherwise.</returns>
		public bool MapToDataStructure(object sourceValue, out object objResult)
		{
			if ((sourceValue == null) || !typeof(decimal).IsAssignableFrom(sourceValue.GetType()))
			{
				objResult = null;
				return false;
			}

			if (definition.SupportedProcessingTypes.Contains(typeof(decimal)))
			{
				objResult = sourceValue;
				return true;
			}

			string strValue = sourceValue.ToString();
			if (!definition.SupportedProcessingTypes.Contains(strValue.GetType()))
			{
				throw new DataMappingException(string.Format("The converted type of a {0} type is not supported.", typeof(decimal).Name));
			}

			objResult = strValue;
			return true;
		}

		/// <summary>
		/// Attempts to map the object to process to an instance of type Decimal, if the target type is a Decimal.
		/// </summary>
		/// <param name="targetType">The target type to process to.</param>
		/// <param name="objToProcess">The object to process to an instance of type Decimal.</param>
		/// <param name="objResult">The result in which the value will be stored.</param>
		/// <returns></returns>
		public bool MapFromDataStructure(Type targetType, object objToProcess, out object objResult)
		{
			if ((targetType == null) || (objToProcess == null) || !typeof(decimal).IsAssignableFrom(targetType))
			{
				objResult = null;
				return false;
			}

			if (objToProcess is decimal)
			{
				objResult = objToProcess;
				return true;
			}

			// At this point, a conversion is needed, but all types other than string will throw an exception.
			if (!(objToProcess is string))
			{
				throw new DataMappingException(string.Format("Only values of type {0} can be used to convert to a {1} value.", typeof(string).Name, typeof(DateTime).Name));
			}

			objResult = decimal.Parse(objToProcess as string);
			return true;
		}

	}
}
