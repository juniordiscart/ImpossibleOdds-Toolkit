namespace ImpossibleOdds
{
	using System;

	public abstract class ImpossibleOddsException : Exception
	{
		public ImpossibleOddsException()
		{ }

		public ImpossibleOddsException(string errMsg)
		: base(errMsg)
		{ }

		public ImpossibleOddsException(string errMsg, params object[] format)
		: base(string.Format(errMsg, format))
		{ }
	}
}
