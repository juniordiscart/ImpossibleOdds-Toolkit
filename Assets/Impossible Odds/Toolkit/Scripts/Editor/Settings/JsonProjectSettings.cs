﻿namespace ImpossibleOdds.Settings
{
	using System.Collections.Generic;
	using UnityEditor;

	public class JsonProjectSettings : AbstractSingleSetting
	{
		private const string JsonUnityTypesAsArray = "IMPOSSIBLE_ODDS_JSON_UNITY_TYPES_AS_ARRAY";

		public JsonProjectSettings(HashSet<string> loadedSymbols)
		: base(loadedSymbols)
		{ }

		public override string SettingName
		{
			get { return "JSON Serialization Settings"; }
		}

		public override string Symbol
		{
			get { return JsonUnityTypesAsArray; }
		}

		public override void DisplayGUI(string searchContext)
		{
			IsSet = EditorGUILayout.ToggleLeft("Serialize Unity values as a JSON array." + (IsChanged ? "*" : string.Empty), IsSet);
			EditorGUILayout.HelpBox(
				IsSet ?
				"Serialize Unity types by default as a JSON array:\n[0, 2, -10]" :
				"Serialize Unity types by default as a JSON object:\n{\n\t\"x\": 0,\n\t\"y\": 2,\n\t\"z\": -10\n}",
				MessageType.None
			);
		}
	}
}
