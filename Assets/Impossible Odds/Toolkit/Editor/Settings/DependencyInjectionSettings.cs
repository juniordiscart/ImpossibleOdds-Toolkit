using UnityEngine;

namespace ImpossibleOdds.Settings
{
	using System.Collections.Generic;
	using UnityEditor;

	public class DependencyInjectionSettings : IProjectSetting
	{
		private const string DisableGlobalScopeAutoInitialization = "IMPOSSIBLE_ODDS_DEPENDENCY_INJECTION_DISABLE_GLOBAL_SCOPE_INITIALIZATION";
		private const string DisableGlobalScopeAutoInjection = "IMPOSSIBLE_ODDS_DEPENDENCY_INJECTION_DISABLE_GLOBAL_AUTO_INJECT";
		
		private HashSet<string> loadedSymbols;
		private Dictionary<string, bool> isSymbolSet = new Dictionary<string, bool>();

		public DependencyInjectionSettings(HashSet<string> loadedSymbols)
		{
			this.loadedSymbols = loadedSymbols;
			isSymbolSet[DisableGlobalScopeAutoInitialization] = loadedSymbols.Contains(DisableGlobalScopeAutoInitialization);
			isSymbolSet[DisableGlobalScopeAutoInjection] = loadedSymbols.Contains(DisableGlobalScopeAutoInjection);
		}

		public bool IsChanged
		{
			get => IsAutoInitializationGlobalScopeChanged | IsAutoInjectionGlobalScopeChanged;
		}

		public string SettingName
		{
			get => "Dependency Injection Settings";
		}

		private bool IsAutoInitializationGlobalScopeChanged
		{
			get => isSymbolSet[DisableGlobalScopeAutoInitialization] != loadedSymbols.Contains(DisableGlobalScopeAutoInitialization);
		}

		private bool IsAutoInjectionGlobalScopeChanged
		{
			get => isSymbolSet[DisableGlobalScopeAutoInjection] != loadedSymbols.Contains(DisableGlobalScopeAutoInjection);
		}

		public void DisplayGUI(string searchContext)
		{
			isSymbolSet[DisableGlobalScopeAutoInitialization] = EditorGUILayout.ToggleLeft("Disable automatic initialization of the global dependency injection scope." + (IsAutoInitializationGlobalScopeChanged ? "*" : string.Empty), isSymbolSet[DisableGlobalScopeAutoInitialization]);
			EditorGUILayout.HelpBox(
				isSymbolSet[DisableGlobalScopeAutoInitialization] ?
					"The global dependency injection scope will not be initialized automatically before the first scene is loaded. The global dependency injection scope will be created automatically when accessing it for the first time." :
					"The global dependency injection scope will be initialized automatically before the first scene is loaded.",
				MessageType.None
			);
			
			GUILayout.Space(10f);
			
			isSymbolSet[DisableGlobalScopeAutoInjection] = EditorGUILayout.ToggleLeft("Disable automatic injection of the global dependency injection scope." + (IsAutoInjectionGlobalScopeChanged ? "*" : string.Empty), isSymbolSet[DisableGlobalScopeAutoInjection]);
			EditorGUILayout.HelpBox(
				isSymbolSet[DisableGlobalScopeAutoInjection] ?
				"The global dependency injection scope will not be injected automatically." :
				"The global dependency injection scope will be injected for each newly loaded scene.",
				MessageType.None
			);
		}

		public void ApplyChanges()
		{
			loadedSymbols.Remove(DisableGlobalScopeAutoInitialization);
			loadedSymbols.Remove(DisableGlobalScopeAutoInjection);

			if (isSymbolSet[DisableGlobalScopeAutoInitialization])
			{
				loadedSymbols.Add(DisableGlobalScopeAutoInitialization);
			}

			if (isSymbolSet[DisableGlobalScopeAutoInjection])
			{
				loadedSymbols.Add(DisableGlobalScopeAutoInjection);
			}
		}
	}
}
