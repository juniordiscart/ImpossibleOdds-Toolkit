using System;

namespace ImpossibleOdds.Weblink
{
	public sealed class WeblinkException : Exception
	{
		public WeblinkException() {}

		public WeblinkException(string message)
		: base(message) {}
	}
}