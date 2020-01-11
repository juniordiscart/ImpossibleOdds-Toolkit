namespace ImpossibleOdds.Serialization.Processors
{
	using System;

	/// <summary>
	/// Interface for deserialization processors.
	/// </summary>
	public interface IDeserializationProcessor : IProcessor
	{
		/// <summary>
		/// Deserialize the data to an instance of the target type that's compatible with the serialization definition.
		/// </summary>
		/// <param name="targetType">The target type to deserialize the given data.</param>
		/// <param name="dataToDeserialize">The data deserialize and apply to the result.</param>
		/// <param name="deserializedResult">The result unto which the data is applied.</param>
		/// <returns>True if deserialization is compatible and accepted, false otherwise.</returns>
		bool Deserialize(Type targetType, object dataToDeserialize, out object deserializedResult);
	}
}
