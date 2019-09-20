namespace ImpossibleOdds
{
	using System;

	public static class ObjectExtensions
	{
		/// <summary>
		/// Throws an ArgumentNullException if the argument is null. Returns the value otherwise.
		/// </summary>
		/// <param name="argument">The argument to check for null.</param>
		/// <param name="argumentName">The name of the argument in case it is null.</param>
		/// <typeparam name="T"></typeparam>
		/// <returns>Returns the argument.</returns>
		public static T ThrowIfNull<T>(this T argument, string argumentName)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(argumentName);
			}

			return argument;
		}
	}
}
