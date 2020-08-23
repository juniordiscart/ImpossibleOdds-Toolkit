namespace ImpossibleOdds.Serialization
{
	using System;
	using System.Collections.Generic;

	using ImpossibleOdds.Serialization.Processors;

	/// <summary>
	/// Bare minimum (de)serialization definition interface.
	/// </summary>
	public interface ISerializationDefinition
	{

		/// <summary>
		/// Sequence of serialization processors.
		/// </summary>
		IEnumerable<ISerializationProcessor> SerializationProcessors
		{
			get;
		}

		/// <summary>
		/// Sequence of deserialization processors.
		/// </summary>
		IEnumerable<IDeserializationProcessor> DeserializationProcessors
		{
			get;
		}

		/// <summary>
		/// Set of types that are natively supported by the serialization format and don't need special action.
		/// </summary>
		HashSet<Type> SupportedTypes
		{
			get;
		}

		/// <summary>
		/// Format provider to process culture-defined settings and formatting.
		/// </summary>
		IFormatProvider FormatProvider
		{
			get;
		}
	}
}
