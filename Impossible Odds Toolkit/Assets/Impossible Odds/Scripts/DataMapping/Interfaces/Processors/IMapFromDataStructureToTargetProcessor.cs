namespace ImpossibleOdds.DataMapping.Processors
{
	/// <summary>
	/// Defines that a processor can map data from a data stream when a target object to be mapped to is already provided.
	/// </summary>
	public interface IMapFromDataStructureToTargetProcessor : IMapFromDataStructureProcessor
	{
		bool MapFromDataStructure(object target, object objToProcess);
	}
}
