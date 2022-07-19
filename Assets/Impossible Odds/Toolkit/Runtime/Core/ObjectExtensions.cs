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
		/// Logs an error when the argument is null.
		/// </summary>
		/// <param name="argument">The argument to test.</param>
		/// <param name="argumentName">The name of the argument. This will be printed in the error message.</param>
		/// <returns>True if an error was logged/the argument is null. False otherwise.</returns>
		public static bool LogErrorIfNull<T>(this T argument, string argumentName)
		{
			if (argument == null)
			{
				if (argument is UnityEngine.Object unityArg)
				{
					Log.Error(unityArg, "Argument '{0}' is null.", argumentName);
				}
				else
				{
					Log.Error("Argument '{0}' is null.", argumentName);
				}

				return true;
			}

			return false;
		}
	}
}
