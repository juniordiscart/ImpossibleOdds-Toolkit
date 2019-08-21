namespace ImpossibleOdds.DataMapping
{
	using System;

	/// <summary>
	/// Exception thrown specifically in data mapping cases.
	/// </summary>
	public sealed class DataMappingException : Exception
	{
		public DataMappingException() { }

		public DataMappingException(string message)
		: base(message) { }
	}
}
