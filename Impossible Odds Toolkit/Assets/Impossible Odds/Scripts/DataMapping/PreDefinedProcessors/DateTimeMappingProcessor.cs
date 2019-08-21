namespace ImpossibleOdds.DataMapping.Processors
{
	using System;
	using System.Globalization;

	/// <summary>
	/// Mapping processing to map DateTime data to and from a string value.
	/// </summary>
	public class DateTimeMappingProcessor : AbstractMappingProcessor, IMapToDataStructureProcessor, IMapFromDataStructureProcessor
	{
		private string dateTimeFormat = string.Empty;

		public DateTimeMappingProcessor(IMappingDefinition definition, string dateTimeFormat = DefaultOptions.DateTimeFormat)
		: base(definition)
		{
			this.dateTimeFormat = dateTimeFormat;
		}

		/// <summary>
		/// Attempt to map the source as a DateTime value to a string value.
		/// </summary>
		/// <param name="sourceValue">The source value as a DateTime value.</param>
		/// <param name="objResult">The result in which the mapped value will be stored.</param>
		/// <returns>True if the mapping is accepted for processing, false otherwise.</returns>
		public bool MapToDataStructure(object sourceValue, out object objResult)
		{
			if ((sourceValue == null) || !typeof(DateTime).IsAssignableFrom(sourceValue.GetType()))
			{
				objResult = null;
				return false;
			}

			if (definition.SupportedProcessingTypes.Contains(typeof(DateTime)))
			{
				objResult = sourceValue;
				return true;
			}

			string strValue = ((DateTime)sourceValue).ToString(dateTimeFormat);
			if (!definition.SupportedProcessingTypes.Contains(strValue.GetType()))
			{
				throw new DataMappingException(string.Format("The converted type of a {0} type is not supported.", typeof(DateTime).Name));
			}

			objResult = strValue;
			return true;
		}

		/// <summary>
		/// Attempt to map the object to process to a valid DateTime value.
		/// </summary>
		/// <param name="targetType">The caller's expected type of the returned result.</param>
		/// <param name="objToProcess">The object to map to a DateTime value.</param>
		/// <param name="objResult">The object in which the result will be stored.</param>
		/// <returns>True if the mapping was accepted for processing, false otherwise.</returns>
		public bool MapFromDataStructure(Type targetType, object objToProcess, out object objResult)
		{
			if ((targetType == null) || (objToProcess == null) || !typeof(DateTime).IsAssignableFrom(targetType))
			{
				objResult = null;
				return false;
			}

			if (objToProcess is DateTime)
			{
				objResult = objToProcess;
				return true;
			}

			// At this point, a conversion is needed, but all types other than string will throw an exception.
			if (!(objToProcess is string))
			{
				throw new DataMappingException(string.Format("Only values of type {0} can be used to convert to a {1} value.", typeof(string).Name, typeof(DateTime).Name));
			}

			objResult = DateTime.ParseExact(objToProcess as string, dateTimeFormat, CultureInfo.InvariantCulture);
			return true;
		}
	}
}
