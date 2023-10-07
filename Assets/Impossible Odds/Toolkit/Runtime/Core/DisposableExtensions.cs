using System;

namespace ImpossibleOdds
{
	public static class DisposableExtensions
	{
		/// <summary>
		/// Disposes of the object, if it isn't null.
		/// </summary>
		/// <param name="argument"></param>
		public static void DisposeIfNotNull(this IDisposable argument)
		{
			if (argument != null)
			{
				argument.Dispose();
			}
		}
	}
}