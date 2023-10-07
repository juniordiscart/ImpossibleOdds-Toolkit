using System.Collections.Generic;
using UnityEditor;

namespace ImpossibleOdds.Settings
{
	public class JsonProjectSettings : AbstractSingleSetting
	{
		private const string JsonUnityTypesAsArray = "IMPOSSIBLE_ODDS_JSON_UNITY_TYPES_AS_ARRAY";

		public JsonProjectSettings(HashSet<string> loadedSymbols)
		: base(loadedSymbols)
		{ }

		public override string SettingName => "JSON Serialization Settings";

		public override string Symbol => JsonUnityTypesAsArray;

		public override void DisplayGUI(string searchContext)
		{
			IsSet = EditorGUILayout.ToggleLeft("Serialize Unity values as a JSON array." + (IsChanged ? "*" : string.Empty), IsSet);
			EditorGUILayout.HelpBox(
				IsSet ?
				"Serialize Unity types by default as a JSON array:\n[0, 2, -10]" :
				"Serialize Unity types by default as a JSON object:\n{\n  \"x\": 0,\n  \"y\": 2,\n  \"z\": -10\n}",
				MessageType.None
			);
		}
	}
}