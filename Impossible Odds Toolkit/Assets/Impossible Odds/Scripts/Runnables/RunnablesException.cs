namespace ImpossibleOdds.Runnables
{
	using System;

	public class RunnablesException : Exception
	{
		public RunnablesException(string errMsg)
		: base (errMsg)
		{ }
	}
}