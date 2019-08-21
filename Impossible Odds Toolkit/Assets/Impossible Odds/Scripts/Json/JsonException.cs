namespace ImpossibleOdds.Json
{
	using System;

	public sealed class JsonException : Exception
	{
		public JsonException(string msg)
		: base(msg) { }
	}
}