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
				throw new ArgumentNullException(argumentName);
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
				throw new ArgumentNullException(argumentName);
			}

			return argument;
		}

		/// <summary>
		/// Logs an error when the argument is null.
		/// </summary>
		/// <param name="argument">The argument to test.</param>
		/// <param name="argumentName">The name of the argument. This will be printed in the error message.</param>
		/// <typeparam name="T"></typeparam>
		/// <returns>True if the argument is null. False otherwise.</returns>
		public static bool LogErrorIfNull<T>(this T argument, string argumentName)
		{
			if (argument == null)
			{
				if (argument is UnityEngine.Object unityArg)
				{
					Debug.Error(unityArg, "Argument '{0}' is null.", argumentName);
				}
				else
				{
					Debug.Error("Argument '{0}' is null.", argumentName);
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Disposes of the object, if it isn't null.
		/// </summary>
		/// <param name="disposable"></param>
		public static void DisposeIfNotNull(this IDisposable argument)
		{
			if (argument != null)
			{
				argument.Dispose();
			}
		}
	}
}
