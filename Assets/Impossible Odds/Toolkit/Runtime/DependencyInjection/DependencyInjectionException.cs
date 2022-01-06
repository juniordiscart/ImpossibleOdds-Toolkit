namespace ImpossibleOdds.DependencyInjection
{
	public sealed class DependencyInjectionException : ImpossibleOddsException
	{
		public DependencyInjectionException()
		{ }

		public DependencyInjectionException(string errMsg)
		: base(errMsg)
		{ }

		public DependencyInjectionException(string errMsg, params object[] format)
		: base(string.Format(errMsg, format))
		{ }
	}
}
