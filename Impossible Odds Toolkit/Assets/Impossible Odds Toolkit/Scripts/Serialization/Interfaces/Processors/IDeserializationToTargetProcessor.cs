namespace ImpossibleOdds.Serialization.Processors
{
	/// <summary>
	/// Defines that a processor can deserialize data directly unto the given instance of the object.
	/// </summary>
	public interface IDeserializationToTargetProcessor : IDeserializationProcessor
	{
		/// <summary>
		/// Directly deserialize the data unto the target.
		/// </summary>
		/// <param name="deserializationTarget">The object on which the data should be applied.</param>
		/// <param name="dataToDeserialize">The data to deserialize.</param>
		/// <returns>True if the deserialization is compatible and accepted, false otherwise.</returns>
		bool Deserialize(object deserializationTarget, object dataToDeserialize);
	}
}
