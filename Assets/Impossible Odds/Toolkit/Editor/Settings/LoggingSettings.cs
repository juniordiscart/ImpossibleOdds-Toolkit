namespace ImpossibleOdds.Settings
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;

	public class LoggingSettings : IProjectSetting
	{
		private static readonly string[] EditorLoggingLevelSymbols = new string[]
		{
			"IMPOSSIBLE_ODDS_LOGGING_EDITOR_INFO",
			"IMPOSSIBLE_ODDS_LOGGING_EDITOR_WARNING",
			"IMPOSSIBLE_ODDS_LOGGING_EDITOR_ERROR"
		};

		private static readonly string[] PlayerLoggingLevelSymbols = new string[]
		{
			"IMPOSSIBLE_ODDS_LOGGING_INFO",
			"IMPOSSIBLE_ODDS_LOGGING_WARNING",
			"IMPOSSIBLE_ODDS_LOGGING_ERROR"
		};

		private static readonly string[] LoggingLevelNames = new string[]
		{
			"Information",
			"Warnings",
			"Errors",
			"Exceptions"
		};

		private static readonly string[] EditorLoggingLevelDescriptions = new string[]
		{
			"Info, warning, error and exception messages are logged in the editor's console.",
			"Warning, error and exception messages are logged in the editor's console. Info messages are suppressed.",
			"Error and exception messages are logged in the editor's console. Info and warning messages are suppressed.",
			"Exception messages are logged in the editor console. Info, warning and error messages are suppressed."
		};

		private static readonly string[] BuildLoggingLevelDescriptions = new string[]
		{
			"Info, warning, error and exception messages are logged in the player's log file.",
			"Warning, error and exception messages are logged in the player's log file. Info messages are suppressed.",
			"Error and exception messages are logged in the player's log file. Info and warning messages are suppressed.",
			"Exception messages are logged in the player's log file. Info, warning and error messages are suppressed."
		};


		[InitializeOnLoadMethod]
		private static void ApplyDefaultLogLevel()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
			{
				return;
			}

			BuildTargetGroup currentTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
			List<string> currentSymbols = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(currentTargetGroup).Split(';'));

			// Check the editor logging level
			if (EditorLoggingLevelSymbols.Join(currentSymbols, key1 => key1, key2 => key2, (key1, key2) => key1).Count() == 0)
			{
				currentSymbols.Add(EditorLoggingLevelSymbols[0]);
			}

			// Check the player logging level
			if (PlayerLoggingLevelSymbols.Join(currentSymbols, key1 => key1, key2 => key2, (key1, key2) => key1).Count() == 0)
			{
				currentSymbols.Add(PlayerLoggingLevelSymbols[0]);
			}

			PlayerSettings.SetScriptingDefineSymbolsForGroup(currentTargetGroup, string.Join(";", currentSymbols));
		}

		private HashSet<string> loadedSymbols = null;

		private int currentBuildLogLevel = 0;
		private int currentEditorLogLevel = 0;

		public bool IsChanged
		{
			get => IsEditorSymbolChanged || IsBuildSymbolChanged;
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
				if (currentBuildLogLevel < PlayerLoggingLevelSymbols.Length)
				{
					return !loadedSymbols.Contains(PlayerLoggingLevelSymbols[currentBuildLogLevel]);
				}
				else
				{
					foreach (string buildLoggingSymbol in PlayerLoggingLevelSymbols)
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

		public string SettingName
		{
			get => "Logging Settings";
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
			for (; currentBuildLogLevel < PlayerLoggingLevelSymbols.Length; ++currentBuildLogLevel)
			{
				if (loadedSymbols.Contains(PlayerLoggingLevelSymbols[currentBuildLogLevel]))
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

			foreach (string buildLoggingSymbol in PlayerLoggingLevelSymbols)
			{
				loadedSymbols.Remove(buildLoggingSymbol);
			}

			// Add the selected symbols
			if (currentEditorLogLevel < EditorLoggingLevelSymbols.Length)
			{
				loadedSymbols.Add(EditorLoggingLevelSymbols[currentEditorLogLevel]);
			}

			if (currentBuildLogLevel < PlayerLoggingLevelSymbols.Length)
			{
				loadedSymbols.Add(PlayerLoggingLevelSymbols[currentBuildLogLevel]);
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
