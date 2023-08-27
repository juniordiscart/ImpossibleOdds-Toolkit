namespace ImpossibleOdds.Serialization.Processors
{
	/// <summary>
	/// Defines that a processor can deserialize data directly unto the given instance of the object.
	/// </summary>
	public interface IDeserializationToTargetProcessor : IDeserializationProcessor
	{
		/// <summary>
		/// Deserialize the provided data directly onto a target object.
		/// </summary>
		/// <param name="deserializationTarget">The object unto which the deserialized data should be applied to.</param>
		/// <param name="dataToDeserialize">The data to deserialize.</param>
		void Deserialize(object deserializationTarget, object dataToDeserialize);
	}
}
