namespace ImpossibleOdds.DataMapping.Processors
{
	using System;

	/// <summary>
	/// Defines that a processor can map data to a data structure.
	/// </summary>
	public interface IMapToDataStructureProcessor : IMappingProcessor
	{
		bool MapToDataStructure(object sourceValue, out object objResult);
	}
}
