using System.Collections.Generic;
using UnityEditor;

namespace ImpossibleOdds.Settings
{
	public class EditorExtensionSettings : AbstractSingleSetting
	{
		private const string EnableCachingSymbol = "IMPOSSIBLE_ODDS_ENABLE_EDITOR_EXT";

		public EditorExtensionSettings(HashSet<string> loadedSymbols) : base(loadedSymbols)
		{ }

		public override string Symbol => EnableCachingSymbol;

		public override string SettingName => "Editor Extensions";

		public override void DisplayGUI(string searchContext)
		{
			IsSet = EditorGUILayout.ToggleLeft("Enable editor extensions." + (IsChanged ? "*" : string.Empty), isSet);
			EditorGUILayout.HelpBox("Editor extensions are disabled by default. Tick the box to enable them.", MessageType.None);
		}
	}
}