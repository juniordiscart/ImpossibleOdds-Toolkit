using System.Collections.Generic;
using UnityEditor;

namespace ImpossibleOdds.Settings
{
	public class DependencyInjectionSettings : AbstractSingleSetting
	{
		private const string GlobalDependencyInjectionDisabled = "IMPOSSIBLE_ODDS_DEPENDENCY_INJECTION_DISABLE_GLOBAL_AUTO_INJECT";

		public DependencyInjectionSettings(HashSet<string> loadedSymbols)
		: base(loadedSymbols)
		{ }

		public override string SettingName => "Dependency Injection Settings";

		public override string Symbol => GlobalDependencyInjectionDisabled;

		public override void DisplayGUI(string searchContext)
		{
			IsSet = EditorGUILayout.ToggleLeft("Disable automatic injection of the global dependency injection scope." + (IsChanged ? "*" : string.Empty), IsSet);
			EditorGUILayout.HelpBox(
				IsSet ?
				"The global dependency injection scope will not be injected automatically." :
				"The global dependency injection scope will be injected for each newly loaded scene.",
				MessageType.None
			);
		}
	}
}