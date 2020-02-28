namespace ImpossibleOdds.Serialization
{
	using System;

	/// <summary>
	/// Exception thrown during (de)serialization.
	/// </summary>
	public sealed class SerializationException : Exception
	{
		public SerializationException() { }

		public SerializationException(string message)
		: base(message) { }
	}
}
