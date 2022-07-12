namespace ImpossibleOdds.Settings
{
	using System.Collections.Generic;
	using UnityEditor;

	public class XmlProjectSettings : AbstractSingleSetting
	{
		private const string XmlUnityTypesAsAttributes = "IMPOSSIBLE_ODDS_XML_UNITY_TYPES_AS_ATTRIBUTES";

		public XmlProjectSettings(HashSet<string> loadedSymbols)
		: base(loadedSymbols)
		{ }

		public override string SettingName
		{
			get => "XML Serialization Settings";
		}

		public override string Symbol
		{
			get => XmlUnityTypesAsAttributes;
		}

		public override void DisplayGUI(string searchContext)
		{
			IsSet = EditorGUILayout.ToggleLeft("Serialize Unity values using XML attributes." + (IsChanged ? "*" : string.Empty), IsSet);
			EditorGUILayout.HelpBox(
				IsSet ?
				"Serialize Unity types by default using XML attributes:\n<position x=\"0\" y=\"2\" z=\"-10\" />" :
				"Serialize Unity types by default using XML elements:\n<position>\n  <x>0</x>\n  <y>2</y>\n  <z>-10</z>\n</position>",
				MessageType.None
			);
		}
	}
}
