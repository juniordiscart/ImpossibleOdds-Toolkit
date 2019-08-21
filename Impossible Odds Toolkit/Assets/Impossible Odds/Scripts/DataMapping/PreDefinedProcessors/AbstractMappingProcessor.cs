namespace ImpossibleOdds.DataMapping.Processors
{
	using System;

	/// <summary>
	/// Base class for mapping processors.
	/// /// </summary>
	public abstract class AbstractMappingProcessor
	{
		protected IMappingDefinition definition;

		public AbstractMappingProcessor(IMappingDefinition definition)
		{
			if (definition == null)
			{
				throw new ArgumentNullException("definition");
			}

			this.definition = definition;
		}
	}
}
