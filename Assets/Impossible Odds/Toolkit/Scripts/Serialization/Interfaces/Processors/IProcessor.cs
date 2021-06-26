namespace ImpossibleOdds.Serialization.Processors
{
	/// <summary>
	/// Denotes that an object can work as a (de)serialization processor.
	/// </summary>
	public interface IProcessor
	{
		/// <summary>
		/// The serialization definition used by the processor to determine which and how it processes the data.
		/// </summary>
		ISerializationDefinition Definition
		{
			get;
		}
	}
}
