namespace ImpossibleOdds.StateMachines
{
	public class StateMachineException : ImpossibleOddsException
	{
		public StateMachineException()
		{ }

		public StateMachineException(string errMsg)
		: base(errMsg)
		{ }

		public StateMachineException(string errMsg, params object[] format)
		: base(string.Format(errMsg, format))
		{ }
	}
}
