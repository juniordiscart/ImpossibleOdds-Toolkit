namespace ImpossibleOdds.DataMapping.Processors
{
	using System;
	using ImpossibleOdds;

	/// <summary>
	/// Base class for mapping processors.
	/// /// </summary>
	public abstract class AbstractMappingProcessor
	{
		protected IMappingDefinition definition;

		public AbstractMappingProcessor(IMappingDefinition definition)
		{
			definition.ThrowIfNull(nameof(definition));
			this.definition = definition;
		}
	}
}
