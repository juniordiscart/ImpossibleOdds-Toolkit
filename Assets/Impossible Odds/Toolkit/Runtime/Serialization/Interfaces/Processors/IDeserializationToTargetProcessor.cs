namespace ImpossibleOdds.Serialization.Processors
{
	/// <summary>
	/// Defines that a processor can deserialize data directly unto the given instance of the object.
	/// </summary>
	public interface IDeserializationToTargetProcessor : IDeserializationProcessor
	{
		/// <summary>
		/// Can this processor deserialize the provided data to the provided object.
		/// </summary>
		/// <param name="deserializationTarget">The target object into which the data should be deserialized and applied to.</param>
		/// <param name="dataToDeserialize">The data to deserialize.</param>
		/// <returns>True if this processor can accept the data, false otherwise.</returns>
		bool CanDeserialize(object deserializationTarget, object dataToDeserialize);
		
		/// <summary>
		/// Deserialize the provided data directly onto a target object.
		/// </summary>
		/// <param name="deserializationTarget">The object to which the deserialized data should be applied to.</param>
		/// <param name="dataToDeserialize">The data to deserialize.</param>
		void Deserialize(object deserializationTarget, object dataToDeserialize);
	}
}
