namespace ImpossibleOdds.Runnables
{
	public class RunnablesException : ImpossibleOddsException
	{
		public RunnablesException()
		{ }

		public RunnablesException(string errMsg)
		: base(errMsg)
		{ }

		public RunnablesException(string errMsg, params object[] format)
		: base(string.Format(errMsg, format))
		{ }
	}
}
