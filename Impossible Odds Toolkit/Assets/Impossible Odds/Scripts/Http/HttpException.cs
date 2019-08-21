namespace ImpossibleOdds.Http
{
	using System;

	public sealed class HttpException : Exception
	{
		public HttpException(string errMsg)
		: base (errMsg) {}
	}
}