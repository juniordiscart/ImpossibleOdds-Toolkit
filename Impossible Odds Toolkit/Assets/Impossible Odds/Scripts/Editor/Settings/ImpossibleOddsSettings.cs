namespace ImpossibleOdds.Settings
{
	using System.IO;
	using System.Linq;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.Compilation;

	[InitializeOnLoad]
	public class ImpossibleOddsSettings : EditorWindow
	{
		private const int currentVersion = 0;

		private const string FirstTimeShowKey = "ImpossidleOdds_Settings_FirstTimeShow";
		private const string LastTargetGroupKey = "ImpossibleOdds_Settings_LastTargetGroup";

		private static BuildTargetGroup loadedSettingsGroup = BuildTargetGroup.Unknown;
		private static HashSet<string> currentSymbols = null;
		private static List<AbstractSetting> settings = null;
		private static Vector2 scrollPosition = Vector2.zero;
		private static readonly Vector2 preferredSize = new Vector2(420, 475);

		static ImpossibleOddsSettings()
		{
#if UNITY_2018
			EditorApplication.projectChanged += EditorUpdate;
			EditorApplication.hierarchyChanged += EditorUpdate;
#else
			EditorApplication.projectWindowChanged += EditorUpdate;
			EditorApplication.hierarchyWindowChanged += EditorUpdate;
#endif
		}

		public static HashSet<string> GetProjectSymbols(BuildTargetGroup targetGroup)
		{
			return new HashSet<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(new char[] { ';' }));
		}

		// [PreferenceItem("Impossible Odds")]
		// private static void ShowSettings()
		// {
		// 	DrawWindow();
		// }

		[MenuItem("Window/Impossible Odds Settings")]
		private static void CreateWindow()
		{
			GetWindow<ImpossibleOddsSettings>(true, "Impossible Odds Settings", true);
		}

		private static void EditorUpdate()
		{
			if (!EditorPrefs.HasKey(FirstTimeShowKey) || (EditorPrefs.GetInt(FirstTimeShowKey) < currentVersion))
			{
				CreateWindow();
				EditorPrefs.SetInt(FirstTimeShowKey, currentVersion);
			}
		}

		private static void DrawWindow()
		{
			if (currentSymbols == null)
			{
				if (EditorPrefs.HasKey(LastTargetGroupKey))
				{
					BuildTargetGroup lastLoadedSettings = (BuildTargetGroup)EditorPrefs.GetInt(LastTargetGroupKey, (int)BuildTargetGroup.Standalone);
					LoadSettingsForGroup(lastLoadedSettings);
				}
				else
				{
					LoadSettingsForGroup(BuildTargetGroup.Standalone);
				}
			}

			EditorGUILayout.BeginVertical();
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

			BuildTargetGroup tempTargetGroup = (BuildTargetGroup)EditorGUILayout.EnumPopup("Build target group", loadedSettingsGroup);
			if (tempTargetGroup != loadedSettingsGroup)
			{
				LoadSettingsForGroup(tempTargetGroup);
			}

			EditorGUILayout.Space();

			foreach (AbstractSetting setting in settings)
			{
				setting.DisplayGUI();
				GUILayout.Space(10);
			}

			EditorGUILayout.EndScrollView();

			EditorGUILayout.Space();
			if (GUILayout.Button("Apply settings", GUILayout.Width(150)))
			{
				ApplySettingsForGroup(loadedSettingsGroup);
			}

			EditorGUILayout.EndVertical();
		}

		private static void LoadSettingsForGroup(BuildTargetGroup targetGroup)
		{
			currentSymbols = GetProjectSymbols(targetGroup);

			settings = new List<AbstractSetting>()
			{
				new EditorExtensionSettings(currentSymbols),
				new LogSettings(currentSymbols),
				new PhotonSettings(currentSymbols)
			};

			loadedSettingsGroup = targetGroup;
			EditorPrefs.SetInt(LastTargetGroupKey, (int)targetGroup);
		}

		private static void ApplySettingsForGroup(BuildTargetGroup targetGroup)
		{
			foreach (AbstractSetting setting in settings)
			{
				setting.ApplySetting();
			}

			// Apply these to the project for the current group
			string symbolStr = string.Join(";", currentSymbols.ToArray());
			PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, symbolStr);
		}

		public ImpossibleOddsSettings()
		{
			this.minSize = preferredSize;
		}

		private void OnGUI()
		{
			GUIStyle bgStyle = new GUIStyle(EditorStyles.label);
			bgStyle.fontSize = 22;
			bgStyle.fontStyle = FontStyle.Bold;
			bgStyle.wordWrap = true;

			EditorGUILayout.BeginVertical();
			GUILayout.Space(10);
			GUILayout.Label("Impossible Odds Settings", bgStyle);
			GUILayout.Space(20);
			EditorGUILayout.EndVertical();

			DrawWindow();

			// EditorGUILayout.BeginVertical();
			// GUILayout.Space(20);
			// GUILayout.Label("You can always change these settings later on by opening the Unity editor preferences.", EditorStyles.wordWrappedLabel);
			// GUILayout.FlexibleSpace();
			// EditorGUILayout.EndVertical();
		}
	}
}
