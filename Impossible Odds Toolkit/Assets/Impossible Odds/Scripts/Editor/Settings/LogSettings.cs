namespace ImpossibleOdds.Settings
{
	using System.Collections.Generic;
	using UnityEditor;

	public class LogSettings : AbstractSetting
	{
		private const string EnableCachingSymbol = "IMPOSSIBLE_ODDS_VERBOSE";

		public LogSettings(HashSet<string> loadedSymbols) : base(loadedSymbols)
		{ }

		public override string Symbol
		{
			get { return EnableCachingSymbol; }
		}

		public override void DisplayGUI()
		{
			isSet = EditorGUILayout.Toggle("Enable logging." + (IsChanged ? "*" : string.Empty), isSet);
			EditorGUILayout.HelpBox("All debug log outputs in this toolkit are disabled by default to not clutter the console output. " +
				"Enable this option to print some additional debug logs.", MessageType.None);
		}
	}
}
