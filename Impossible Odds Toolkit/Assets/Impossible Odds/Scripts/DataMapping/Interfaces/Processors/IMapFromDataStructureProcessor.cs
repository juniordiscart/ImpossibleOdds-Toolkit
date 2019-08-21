namespace ImpossibleOdds.DataMapping.Processors
{
	using System;

	/// <summary>
	/// Defines that a processor can map data from a data stream.
	/// </summary>
	public interface IMapFromDataStructureProcessor : IMappingProcessor
	{
		bool MapFromDataStructure(Type targetType, object objToProcess, out object objResult);
	}
}
