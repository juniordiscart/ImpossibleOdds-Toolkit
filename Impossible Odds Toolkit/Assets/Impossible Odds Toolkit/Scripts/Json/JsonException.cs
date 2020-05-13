namespace ImpossibleOdds.Json
{
	public sealed class JsonException : ImpossibleOddsException
	{
		public JsonException()
		{ }

		public JsonException(string errMsg)
		: base(errMsg)
		{ }

		public JsonException(string errMsg, params object[] format)
		: base(string.Format(errMsg, format))
		{ }
	}
}
