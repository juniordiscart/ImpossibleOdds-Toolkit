namespace ImpossibleOdds.Settings
{
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	public class LoggingSettings : IProjectSetting
	{
		private HashSet<string> loadedSymbols = null;

		private int currentBuildLogLevel = 0;
		private int currentEditorLogLevel = 0;

		private readonly string[] EditorLoggingLevelSymbols = new string[]
		{
			"IMPOSSIBLE_ODDS_LOGGING_EDITOR_INFO",
			"IMPOSSIBLE_ODDS_LOGGING_EDITOR_WARNING",
			"IMPOSSIBLE_ODDS_LOGGING_EDITOR_ERROR"
		};

		private readonly string[] BuildLoggingLevelSymbols = new string[]
		{
			"IMPOSSIBLE_ODDS_LOGGING_INFO",
			"IMPOSSIBLE_ODDS_LOGGING_WARNING",
			"IMPOSSIBLE_ODDS_LOGGING_ERROR"
		};

		private readonly string[] LoggingLevelNames = new string[]
		{
			"Information",
			"Warnings",
			"Errors",
			"Exceptions"
		};

		private readonly string[] EditorLoggingLevelDescriptions = new string[]
		{
			"Info, warning, error and exception messages are logged in the editor's console.",
			"Warning, error and exception messages are logged in the editor's console. Info messages are suppressed.",
			"Error and exception messages are logged in the editor's console. Info and warning messages are suppressed.",
			"Exception messages are logged in the editor console. Info, warning and error messages are suppressed."
		};

		private readonly string[] BuildLoggingLevelDescriptions = new string[]
		{
			"Info, warning, error and exception messages are logged in the player's log file.",
			"Warning, error and exception messages are logged in the player's log file. Info messages are suppressed.",
			"Error and exception messages are logged in the player's log file. Info and warning messages are suppressed.",
			"Exception messages are logged in the player's log file. Info, warning and error messages are suppressed."
		};

		public bool IsChanged
		{
			get { return IsEditorSymbolChanged || IsBuildSymbolChanged; }
		}

		public bool IsEditorSymbolChanged
		{
			get
			{
				if (currentEditorLogLevel < EditorLoggingLevelSymbols.Length)
				{
					return !loadedSymbols.Contains(EditorLoggingLevelSymbols[currentEditorLogLevel]);
				}
				else
				{
					foreach (string editorLoggingSymbol in EditorLoggingLevelSymbols)
					{
						if (loadedSymbols.Contains(editorLoggingSymbol))
						{
							return true;
						}
					}

					return false;
				}
			}
		}

		public bool IsBuildSymbolChanged
		{
			get
			{
				if (currentBuildLogLevel < BuildLoggingLevelSymbols.Length)
				{
					return !loadedSymbols.Contains(BuildLoggingLevelSymbols[currentBuildLogLevel]);
				}
				else
				{
					foreach (string buildLoggingSymbol in BuildLoggingLevelSymbols)
					{
						if (loadedSymbols.Contains(buildLoggingSymbol))
						{
							return true;
						}
					}

					return false;
				}
			}
		}

		public LoggingSettings(HashSet<string> loadedSymbols)
		{
			this.loadedSymbols = loadedSymbols;

			// Check current editor logging level
			for (; currentEditorLogLevel < EditorLoggingLevelSymbols.Length; ++currentEditorLogLevel)
			{
				if (loadedSymbols.Contains(EditorLoggingLevelSymbols[currentEditorLogLevel]))
				{
					break;
				}
			}

			// Check current build logging level
			for (; currentBuildLogLevel < BuildLoggingLevelSymbols.Length; ++currentBuildLogLevel)
			{
				if (loadedSymbols.Contains(BuildLoggingLevelSymbols[currentBuildLogLevel]))
				{
					break;
				}
			}
		}

		public void ApplyChanges()
		{
			// Remove any editor and player symbols first
			foreach (string editorLoggingSymbol in EditorLoggingLevelSymbols)
			{
				loadedSymbols.Remove(editorLoggingSymbol);
			}

			foreach (string buildLoggingSymbol in BuildLoggingLevelSymbols)
			{
				loadedSymbols.Remove(buildLoggingSymbol);
			}

			// Add the selected symbols
			if (currentEditorLogLevel < EditorLoggingLevelSymbols.Length)
			{
				loadedSymbols.Add(EditorLoggingLevelSymbols[currentEditorLogLevel]);
			}

			if (currentBuildLogLevel < BuildLoggingLevelSymbols.Length)
			{
				loadedSymbols.Add(BuildLoggingLevelSymbols[currentBuildLogLevel]);
			}
		}

		public void DisplayGUI(string searchContext)
		{
			// TODO: display asterisk when changed
			currentEditorLogLevel = EditorGUILayout.Popup("Editor logging level." + (IsEditorSymbolChanged ? "*" : string.Empty), currentEditorLogLevel, LoggingLevelNames);
			EditorGUILayout.HelpBox(EditorLoggingLevelDescriptions[currentEditorLogLevel], MessageType.None);

			GUILayout.Space(ProjectSettings.DefaultSettingSpacing);

			// TODO: display asterisk when changed
			currentBuildLogLevel = EditorGUILayout.Popup("Player logging level." + (IsBuildSymbolChanged ? "*" : string.Empty), currentBuildLogLevel, LoggingLevelNames);
			EditorGUILayout.HelpBox(BuildLoggingLevelDescriptions[currentBuildLogLevel], MessageType.None);
		}
	}
}
