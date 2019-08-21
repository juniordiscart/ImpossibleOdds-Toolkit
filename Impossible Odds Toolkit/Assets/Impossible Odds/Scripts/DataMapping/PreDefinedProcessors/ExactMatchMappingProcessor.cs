namespace ImpossibleOdds.DataMapping.Processors
{
	using System;

	/// <summary>
	/// Simple mapping processor that checks whether the values are directly assignable and supported by the mapping definition.
	/// </summary>
	public class ExactMatchMappingProcessor : AbstractMappingProcessor, IMapToDataStructureProcessor, IMapFromDataStructureProcessor
	{
		public ExactMatchMappingProcessor(IMappingDefinition definition)
		: base(definition)
		{ }

		/// <summary>
		/// Attempts to map an object to a directly supported type by the mapping definition.
		/// </summary>
		/// <param name="sourceValue">The source value to check whether it is directly supported in the mapping definition.</param>
		/// <param name="objResult">The result in which the value will be stored.</param>
		/// <returns>True if the mapping is accepted for processing, false otherwise.</returns>
		public bool MapToDataStructure(object sourceValue, out object objResult)
		{
			if ((sourceValue == null) || !definition.SupportedProcessingTypes.Contains(sourceValue.GetType()))
			{
				objResult = null;
				return false;
			}

			objResult = sourceValue;
			return true;
		}

		/// <summary>
		/// Attempts to map the object to process directly to an instance of the requested type.
		/// </summary>
		/// <param name="targetType">The expected type the result will be processed to.</param>
		/// <param name="objToProcess">The object to process.</param>
		/// <param name="objResult">The result in which the value will be stored.</param>
		/// <returns>True if the mapping is accepted for processing, false otherwise.</returns>
		public bool MapFromDataStructure(Type targetType, object objToProcess, out object objResult)
		{
			if (targetType == null)
			{
				objResult = null;
				return false;
			}

			// If the target type is nullable and we're dealing with a null value, then lets just call it quits.
			if ((objToProcess == null) && !targetType.IsValueType)
			{
				objResult = objToProcess;
				return true;
			}

			if (!targetType.IsAssignableFrom(objToProcess.GetType()))
			{
				objResult = null;
				return false;
			}

			objResult = objToProcess;
			return true;
		}
	}
}
