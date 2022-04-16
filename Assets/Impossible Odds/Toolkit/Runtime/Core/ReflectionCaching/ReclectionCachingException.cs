namespace ImpossibleOdds.ReflectionCaching
{
	public class ReflectionCachingException : ImpossibleOddsException
	{
		public ReflectionCachingException()
		{ }

		public ReflectionCachingException(string errMsg) : base(errMsg)
		{ }

		public ReflectionCachingException(string errMsg, params object[] format) : base(errMsg, format)
		{ }
	}
}
