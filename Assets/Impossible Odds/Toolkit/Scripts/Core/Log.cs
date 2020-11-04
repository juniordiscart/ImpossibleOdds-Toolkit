namespace ImpossibleOdds
{
	using System;
	using System.Diagnostics;

	/// <summary>
	/// Adds conditional logging and pipes them through to Unity's Debug functions.
	/// </summary>
	public static class Log
	{
		/// <summary>
		/// Log an information message to the Unity console.
		/// </summary>
		/// <param name="context">The Unity object context for this message.</param>
		/// <param name="message">The message to log to console.</param>
		/// <param name="additionalParams">Additional values to be formatted in the message.</param>
#if UNITY_EDITOR
		[Conditional("IMPOSSIBLE_ODDS_LOGGING_EDITOR_INFO")]
#else
		[Conditional("IMPOSSIBLE_ODDS_LOGGING_INFO")]
#endif
		public static void Info(UnityEngine.Object context, string message, params object[] additionalParams)
		{
			context.ThrowIfNull(nameof(context));
			UnityEngine.Debug.LogFormat(context, message, additionalParams);
		}

		/// <summary>
		/// Log an information message to the Unity console.
		/// </summary>
		/// <param name="message">The message to log to console.</param>
		/// <param name="additionalParams">Additional values to be formatted in the message.</param>
#if UNITY_EDITOR
		[Conditional("IMPOSSIBLE_ODDS_LOGGING_EDITOR_INFO")]
#else
		[Conditional("IMPOSSIBLE_ODDS_LOGGING_INFO")]
#endif
		public static void Info(string message, params object[] additionalParams)
		{
			UnityEngine.Debug.LogFormat(message, additionalParams);
		}

		/// <summary>
		/// Log an information message to the Unity console.
		/// </summary>
		/// <param name="message">The message to log to console.</param>
#if UNITY_EDITOR
		[Conditional("IMPOSSIBLE_ODDS_LOGGING_EDITOR_INFO")]
#else
		[Conditional("IMPOSSIBLE_ODDS_LOGGING_INFO")]
#endif
		public static void Info(string message)
		{
			UnityEngine.Debug.Log(message);
		}

		/// <summary>
		/// Log a warning message to the Unity console.
		/// </summary>
		/// <param name="context">The Unity object context for this message.</param>
		/// <param name="message">The message to log to console.</param>
		/// <param name="additionalParams">Additional values to be formatted in the message.</param>
#if UNITY_EDITOR
		[Conditional("IMPOSSIBLE_ODDS_LOGGING_EDITOR_INFO"),
		Conditional("IMPOSSIBLE_ODDS_LOGGING_EDITOR_WARNING")]
#else
		[Conditional("IMPOSSIBLE_ODDS_LOGGING_INFO"),
		Conditional("IMPOSSIBLE_ODDS_LOGGING_WARNING")]
#endif
		public static void Warning(UnityEngine.Object context, string message, params object[] additionalParams)
		{
			context.ThrowIfNull(nameof(context));
			UnityEngine.Debug.LogWarningFormat(context, message, additionalParams);
		}

		/// <summary>
		/// Log a warning message to the Unity console.
		/// </summary>
		/// <param name="message">The message to log to console.</param>
		/// <param name="additionalParams">Additional values to be formatted in the message.</param>
#if UNITY_EDITOR
		[Conditional("IMPOSSIBLE_ODDS_LOGGING_EDITOR_INFO"),
		Conditional("IMPOSSIBLE_ODDS_LOGGING_EDITOR_WARNING")]
#else
		[Conditional("IMPOSSIBLE_ODDS_LOGGING_INFO"),
		Conditional("IMPOSSIBLE_ODDS_LOGGING_WARNING")]
#endif
		public static void Warning(string message, params object[] additionalParams)
		{
			UnityEngine.Debug.LogWarningFormat(message, additionalParams);
		}

		/// <summary>
		/// Log a warning message to the Unity console.
		/// </summary>
		/// <param name="message">The message to log to console.</param>
#if UNITY_EDITOR
		[Conditional("IMPOSSIBLE_ODDS_LOGGING_EDITOR_INFO"),
		Conditional("IMPOSSIBLE_ODDS_LOGGING_EDITOR_WARNING")]
#else
		[Conditional("IMPOSSIBLE_ODDS_LOGGING_INFO"),
		Conditional("IMPOSSIBLE_ODDS_LOGGING_WARNING")]
#endif
		public static void Warning(string message)
		{
			UnityEngine.Debug.LogWarning(message);
		}

		/// <summary>
		/// Log an error message to the Unity console.
		/// </summary>
		/// <param name="context">The Unity object context for this message.</param>
		/// <param name="message">The message to log to console.</param>
		/// <param name="additionalParams">Additional values to be formatted in the message.</param>
#if UNITY_EDITOR
		[Conditional("IMPOSSIBLE_ODDS_LOGGING_EDITOR_INFO"),
		Conditional("IMPOSSIBLE_ODDS_LOGGING_EDITOR_WARNING"),
		Conditional("IMPOSSIBLE_ODDS_LOGGING_EDITOR_ERROR")]
#else
		[Conditional("IMPOSSIBLE_ODDS_LOGGING_INFO"),
		Conditional("IMPOSSIBLE_ODDS_LOGGING_WARNING"),
		Conditional("IMPOSSIBLE_ODDS_LOGGING_ERROR")]
#endif
		public static void Error(UnityEngine.Object context, string message, params object[] additionalParams)
		{
			context.ThrowIfNull(nameof(context));
			UnityEngine.Debug.LogErrorFormat(context, message, additionalParams);
		}

		/// <summary>
		/// Log an error message to the Unity console.
		/// </summary>
		/// <param name="message">The message to log to console.</param>
		/// <param name="additionalParams">Additional values to be formatted in the message.</param>
#if UNITY_EDITOR
		[Conditional("IMPOSSIBLE_ODDS_LOGGING_EDITOR_INFO"),
		Conditional("IMPOSSIBLE_ODDS_LOGGING_EDITOR_WARNING"),
		Conditional("IMPOSSIBLE_ODDS_LOGGING_EDITOR_ERROR")]
#else
		[Conditional("IMPOSSIBLE_ODDS_LOGGING_INFO"),
		Conditional("IMPOSSIBLE_ODDS_LOGGING_WARNING"),
		Conditional("IMPOSSIBLE_ODDS_LOGGING_ERROR")]
#endif
		public static void Error(string message, params object[] additionalParams)
		{
			UnityEngine.Debug.LogErrorFormat(message, additionalParams);
		}

		/// <summary>
		/// Log an error message to the Unity console.
		/// </summary>
		/// <param name="message">The message to log to console.</param>
#if UNITY_EDITOR
		[Conditional("IMPOSSIBLE_ODDS_LOGGING_EDITOR_INFO"),
		Conditional("IMPOSSIBLE_ODDS_LOGGING_EDITOR_WARNING"),
		Conditional("IMPOSSIBLE_ODDS_LOGGING_EDITOR_ERROR")]
#else
		[Conditional("IMPOSSIBLE_ODDS_LOGGING_INFO"),
		Conditional("IMPOSSIBLE_ODDS_LOGGING_WARNING"),
		Conditional("IMPOSSIBLE_ODDS_LOGGING_ERROR")]
#endif
		public static void Error(string message)
		{
			UnityEngine.Debug.LogError(message);
		}

		/// <summary>
		/// Log an exception to the Unity console.
		/// </summary>
		/// <param name="e">The exception to log.</param>
		// [Conditional("LOG_LEVEL_EXCEPTION"), Conditional("EDITOR_LOG_LEVEL_EXCEPTION")]
		public static void Exception(Exception e)
		{
			UnityEngine.Debug.LogException(e);
		}

		/// <summary>
		/// Log an exception to the Unity console.
		/// </summary>
		/// <param name="context">The Unity object context for this exception.</param>
		/// <param name="e">The exception to log.</param>
		// [Conditional("LOG_LEVEL_EXCEPTION"), Conditional("EDITOR_LOG_LEVEL_EXCEPTION")]
		public static void Exception(UnityEngine.Object context, Exception e)
		{
			context.ThrowIfNull(nameof(context));
			UnityEngine.Debug.LogException(e, context);
		}
	}
}
