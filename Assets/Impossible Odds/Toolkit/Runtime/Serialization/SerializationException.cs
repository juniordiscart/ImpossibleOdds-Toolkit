namespace ImpossibleOdds.Serialization
{
	public sealed class SerializationException : ImpossibleOddsException
	{
		public SerializationException()
		{ }

		public SerializationException(string errMsg)
		: base(errMsg)
		{ }

		public SerializationException(string errMsg, params object[] format)
		: base(string.Format(errMsg, format))
		{ }
	}
}
