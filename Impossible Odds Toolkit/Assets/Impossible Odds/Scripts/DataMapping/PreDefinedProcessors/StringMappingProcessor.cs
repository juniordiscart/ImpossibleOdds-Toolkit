namespace ImpossibleOdds.DataMapping.Processors
{
	using System;

	/// <summary>
	/// Simple mapping processor to insert a string value in the data structure.
	/// </summary>
	public class StringMappingProcessor : AbstractMappingProcessor, IMapFromDataStructureProcessor
	{
		public StringMappingProcessor(IMappingDefinition definition)
		: base(definition)
		{ }

		/// <summary>
		/// Attempts to map the given value to a string.
		/// </summary>
		/// <param name="targetType">The caller's expected type of the processed result. This processor will only return string values.</param>
		/// <param name="objToProcess">The value to process to a string.</param>
		/// <param name="objResult">The result is stored in this object.</param>
		/// <returns>True if the mapping is accepted for processing, false otherwise.</returns>
		public bool MapFromDataStructure(Type targetType, object objToProcess, out object objResult)
		{
			if (!typeof(string).IsAssignableFrom(targetType))
			{
				objResult = null;
				return false;
			}

			if (objToProcess is string)
			{
				objResult = objToProcess;
				return true;
			}

			objResult = Convert.ChangeType(objToProcess, targetType);
			return true;
		}
	}
}
