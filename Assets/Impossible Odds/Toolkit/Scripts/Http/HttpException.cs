namespace ImpossibleOdds.Http
{
	public sealed class HttpException : ImpossibleOddsException
	{
		public HttpException()
		{ }

		public HttpException(string errMsg)
		: base(errMsg)
		{ }

		public HttpException(string errMsg, params object[] format)
		: base(string.Format(errMsg, format))
		{ }
	}
}
