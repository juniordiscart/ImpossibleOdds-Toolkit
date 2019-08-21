namespace ImpossibleOdds.DependencyInjection
{
	using System;

	public sealed class DependencyInjectionException : Exception
	{
		public DependencyInjectionException(string errMsg)
		: base(errMsg)
		{ }
	}
}