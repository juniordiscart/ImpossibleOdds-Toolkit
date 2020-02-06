namespace ImpossibleOdds.Settings
{
	using System.Collections.Generic;
	using UnityEditor;

	public class EditorExtensionSettings : AbstractSingleSetting
	{
		private const string EnableCachingSymbol = "IMPOSSIBLE_ODDS_DISABLE_EDITOR_EXT";

		public EditorExtensionSettings(HashSet<string> loadedSymbols) : base(loadedSymbols)
		{ }

		public override string Symbol
		{
			get { return EnableCachingSymbol; }
		}

		public override void DisplayGUI(string searchContext)
		{
			isSet = EditorGUILayout.Toggle("Disable editor extensions." + (IsChanged ? "*" : string.Empty), isSet);
			EditorGUILayout.HelpBox("Editor extensions are enabled by default. Tick the box to disable them.", MessageType.None);
		}
	}
}
