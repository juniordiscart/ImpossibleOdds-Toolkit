namespace ImpossibleOdds.DataMapping
{
	using System;
	using System.Collections.Generic;

	using ImpossibleOdds.DataMapping.Processors;

	/// <summary>
	/// Mapping definition essentials.
	/// </summary>
	public interface IMappingDefinition
	{

		/// <summary>
		/// Sequence of mapping processors for mapping to a data structure.
		/// </summary>
		/// <value></value>
		IEnumerable<IMapToDataStructureProcessor> ToDataStructureProcessors
		{
			get;
		}

		/// <summary>
		/// Sequence of mapping processors for mapping from a data structure.
		/// </summary>
		/// <value></value>
		IEnumerable<IMapFromDataStructureProcessor> FromDataStructureProcessors
		{
			get;
		}

		/// <summary>
		/// Set of types that are natively supported by the data structure and don't need any transformation mechanisms applied to them.
		/// </summary>
		/// <value></value>
		HashSet<Type> SupportedProcessingTypes
		{
			get;
		}
	}
}
