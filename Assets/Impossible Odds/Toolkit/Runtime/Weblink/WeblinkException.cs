namespace ImpossibleOdds.Weblink
{
	public sealed class WeblinkException : ImpossibleOddsException
	{
		public WeblinkException()
		{ }

		public WeblinkException(string errMsg)
		: base(errMsg)
		{ }

		public WeblinkException(string errMsg, params object[] format)
		: base(string.Format(errMsg, format))
		{ }
	}
}
