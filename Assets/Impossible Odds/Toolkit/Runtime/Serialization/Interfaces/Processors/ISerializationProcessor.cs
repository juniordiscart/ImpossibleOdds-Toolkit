namespace ImpossibleOdds.Serialization.Processors
{
	/// <summary>
	/// Interface for serialization processors.
	/// </summary>
	public interface ISerializationProcessor : IProcessor
	{

		/// <summary>
		/// Serialize the object to a data structure that's compatible with the serialization definition.
		/// </summary>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <param name="serializedResult">The serialized object.</param>
		/// <returns>True if the serialization is compatible and accepted, false otherwise.</returns>
		bool Serialize(object objectToSerialize, out object serializedResult);
	}
}
