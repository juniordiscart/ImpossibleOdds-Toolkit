using System;

namespace ImpossibleOdds.Serialization.Processors
{
	/// <summary>
	/// Interface for deserialization processors.
	/// </summary>
	public interface IDeserializationProcessor : IProcessor
	{
		/// <summary>
		/// Can this processor deserialize the provided object to an instance of the target type?
		/// </summary>
		/// <param name="targetType">The target type to deserialize the given data to.</param>
		/// <param name="dataToDeserialize">The data to deserialize.</param>
		/// <returns>True if this processor can accept the data, false otherwise.</returns>
		bool CanDeserialize(Type targetType, object dataToDeserialize);

		/// <summary>
		/// Deserialize the provided data to an instance of the target type.
		/// Calling this assumes the processor is capable of processing the data to the target type.
		/// </summary>
		/// <param name="targetType">The target type to which the data should be deserialized.</param>
		/// <param name="dataToDeserialize">The data to deserialize.</param>
		/// <returns>An instance of the target type with the deserialized data applied to it.</returns>
		object Deserialize(Type targetType, object dataToDeserialize);
	}
}