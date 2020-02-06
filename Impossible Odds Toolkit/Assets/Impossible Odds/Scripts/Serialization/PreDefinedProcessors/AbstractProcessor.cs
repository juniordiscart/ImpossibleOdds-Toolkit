namespace ImpossibleOdds.Serialization.Processors
{
	/// <summary>
	/// Base class for (de)serialization processors.
	/// </summary>
	public abstract class AbstractProcessor
	{
		protected ISerializationDefinition definition;

		public AbstractProcessor(ISerializationDefinition definition)
		{
			definition.ThrowIfNull(nameof(definition));
			this.definition = definition;
		}
	}
}
