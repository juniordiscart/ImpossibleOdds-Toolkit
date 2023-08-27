namespace ImpossibleOdds.Serialization.Processors
{
	/// <summary>
	/// Interface for serialization processors.
	/// </summary>
	public interface ISerializationProcessor : IProcessor
	{
		/// <summary>
		/// Can this processor serialize the provided object to a data format supported by the serialization definition?
		/// </summary>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <returns>True if this processor can accept the data, false otherwise.</returns>
		bool CanSerialize(object objectToSerialize);

		/// <summary>
		/// Serialize the provided object, to a result that's supported by the processor's serialization definition.
		/// Calling this assumes the processor is capable of processing the data to a supported format.
		/// </summary>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <returns>The serialized result as supported by the serialization definition.</returns>
		object Serialize(object objectToSerialize);
	}
}
