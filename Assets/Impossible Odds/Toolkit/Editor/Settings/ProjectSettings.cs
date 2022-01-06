namespace ImpossibleOdds.Settings
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;

	public class ProjectSettings : SettingsProvider
	{
		[SettingsProvider]
		public static SettingsProvider CreateProvider()
		{
			return new ProjectSettings();
		}

		public static HashSet<string> GetProjectSymbols(BuildTargetGroup targetGroup)
		{
			return new HashSet<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(new char[] { ';' }));
		}

		public const float DefaultSettingSpacing = 20f;
		private const string ProjectSettingsPath = "Project/Impossible Odds";

		private BuildTargetGroup loadedSettingsGroup = BuildTargetGroup.Unknown;
		private HashSet<string> currentSymbols = null;
		private List<IProjectSetting> settings = null;
		private Vector2 scrollPosition = Vector2.zero;

		public ProjectSettings()
		: base(ProjectSettingsPath, SettingsScope.Project)
		{
			loadedSettingsGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
			LoadSettingsForGroup(loadedSettingsGroup);
		}

		public override void OnGUI(string searchContext)
		{
			DrawWindow(searchContext);
		}

		private void DrawWindow(string searchContext)
		{
			EditorGUILayout.BeginVertical();
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

			DrawBuildTargetSettings(searchContext);
			EditorGUILayout.EndScrollView();

			EditorGUILayout.EndVertical();
		}

		private void DrawBuildTargetSettings(string searchContext)
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			EditorGUILayout.LabelField("Build Target Settings", EditorStyles.boldLabel);
			EditorGUILayout.Space();

			BuildTargetGroup tempTargetGroup = (BuildTargetGroup)EditorGUILayout.EnumPopup("Target group", loadedSettingsGroup);
			EditorGUILayout.HelpBox("These settings all apply for the selected build target. Please review these settings per desired platform.", MessageType.None);
			if (tempTargetGroup != loadedSettingsGroup)
			{
				LoadSettingsForGroup(tempTargetGroup);
			}

			GUILayout.Space(20f);

			foreach (IProjectSetting setting in settings)
			{
				GUILayout.Label(setting.SettingName, EditorStyles.boldLabel);
				setting.DisplayGUI(searchContext);

				if (settings.Last() != setting)
				{
					GUILayout.Space(20f);
				}
			}

			EditorGUILayout.Space(50f);

			if (GUILayout.Button("Apply", GUILayout.Width(150)))
			{
				ApplySettingsForGroup(loadedSettingsGroup);
			}
			EditorGUILayout.EndVertical();
		}

		private void LoadSettingsForGroup(BuildTargetGroup targetGroup)
		{
			currentSymbols = GetProjectSymbols(targetGroup);

			settings = new List<IProjectSetting>()
			{
				new LoggingSettings(currentSymbols),
				new DependencyInjectionSettings(currentSymbols),
				new JsonProjectSettings(currentSymbols),
				new XmlProjectSettings(currentSymbols),
				new EditorExtensionSettings(currentSymbols),
			};

			loadedSettingsGroup = targetGroup;
		}

		private void ApplySettingsForGroup(BuildTargetGroup targetGroup)
		{
			foreach (IProjectSetting setting in settings)
			{
				setting.ApplyChanges();
			}

			// Apply these to the project for the current group
			string symbolStr = string.Join(";", currentSymbols.ToArray());
			PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, symbolStr);
		}
	}
}
