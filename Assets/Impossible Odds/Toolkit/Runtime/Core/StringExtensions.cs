namespace ImpossibleOdds
{
	using System;

	public static class StringExtensions
	{
		/// <summary>
		/// Throws an ArgumentNullException if the string is null or empty. Returns the string otherwise.
		/// </summary>
		/// <param name="argument">The string to check for null or emptiness.</param>
		/// <param name="argumentName">The name of the string in case it is null or empty.</param>
		/// <returns>Returns the string.</returns>
		public static string ThrowIfNullOrEmpty(this string argument, string argumentName)
		{
			if (string.IsNullOrEmpty(argument))
			{
				throw (argument == null) ? new ArgumentNullException(argumentName) : new ArgumentException(argumentName);
			}

			return argument;
		}

		/// <summary>
		/// Throws an ArgumentNullException if the string is null or only contains whitespace characters. Returns the string otherwise.
		/// </summary>
		/// <param name="argument">The string to check for null or whitespaces.</param>
		/// <param name="argumentName">The name of the string in case it is null or just whitespace.</param>
		/// <returns>Returns the string.</returns>
		public static string ThrowIfNullOrWhitespace(this string argument, string argumentName)
		{
			if (string.IsNullOrWhiteSpace(argument))
			{
				throw (argument == null) ? new ArgumentNullException(argumentName) : new ArgumentException(argumentName);
			}

			return argument;
		}

		/// <summary>
		/// Logs an error when the argument is null or empty.
		/// </summary>
		/// <param name="argument">The argument to test.</param>
		/// <param name="argumentName">The name of the argument. This will be printed in the error message.</param>
		/// <returns>True if an error was logged/the argument is null or empty. False otherwise.</returns>
		public static bool LogErrorIfNullOrEmpty(this string argument, string argumentName)
		{
			if (string.IsNullOrEmpty(argument))
			{
				Log.Error("Agurment '{0}' is {1}.", argumentName, ((argument == null) ? "null" : "empty"));
				return true;
			}

			return false;
		}

		/// <summary>
		/// Logs an error when the argument is null or just whitespace characters.
		/// </summary>
		/// <param name="argument">The argument to test.</param>
		/// <param name="argumentName">The name of the argument. This will be printed in the error message.</param>
		/// <returns>True if an error was logged/the argument is null or whitespace. False otherwise.</returns>
		public static bool LogErrorIfNullOrWhitespace(this string argument, string argumentName)
		{
			if (string.IsNullOrWhiteSpace(argument))
			{
				Log.Error("Agurment '{0}' is {1}.", argumentName, ((argument == null) ? "null" : "whitespace"));
				return true;
			}

			return false;
		}

		/// <summary>
		/// Checks whether the string is either null, or empty.
		/// </summary>
		/// <param name="s"></param>
		/// <returns>True if the string is either null or empty. False otherwise.</returns>
		public static bool IsNullOrEmpty(this string s)
		{
			return string.IsNullOrEmpty(s);
		}

		/// <summary>
		/// Checks whether the string is either null, empty, or only consists of whitespace characters.
		/// </summary>
		/// <param name="s"></param>
		/// <returns>True if the string is either null, empty or only whitespace characters. False otherwise.</returns>
		public static bool IsNullOrWhiteSpace(this string s)
		{
			return string.IsNullOrWhiteSpace(s);
		}
	}
}
