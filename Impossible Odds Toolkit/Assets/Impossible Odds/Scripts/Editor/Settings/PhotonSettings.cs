namespace ImpossibleOdds.Settings
{
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.Compilation;
	using System.IO;
	using System.Linq;
	using System.Collections.Generic;

	public class PhotonSettings : AbstractSetting
	{
		private const string PhotonRealTimeAssembly = "PhotonRealtime";
		private const string PhotonUnityAssembly = "PhotonUnityNetworking";
		private const string PhotonModuleGUID = "c94722c414c4f469a89054892e056063";

		private const string EnableCachingSymbol = "IMPOSSIBLE_ODDS_PHOTON";

		private bool isLocked = false;

		public PhotonSettings(HashSet<string> loadedSymbols) : base(loadedSymbols)
		{
			isLocked = !IsPhotonInstalled();
		}

		public override string Symbol
		{
			get { return EnableCachingSymbol; }
		}

		public override void DisplayGUI()
		{
			EditorGUI.BeginDisabledGroup(isLocked);
			isSet = EditorGUILayout.Toggle("Enable Photon module." + (IsChanged ? "*" : string.Empty), isSet);
			EditorGUILayout.HelpBox("Enable this option to use additional Photon-based tools available in this toolkit.", MessageType.None);
			EditorGUI.EndDisabledGroup();

			if (isLocked)
			{
				EditorGUILayout.HelpBox("Photon is not installed. Please install the Photon Unity Networking package to change this setting.", MessageType.Info);
				if (GUILayout.Button("Go to site", GUILayout.Width(150)))
				{
					Application.OpenURL("https://www.photonengine.com");
				}
			}
		}

		public override void ApplySetting()
		{
			base.ApplySetting();
			UpdatePhotonModuleReferences();
		}

		private bool IsPhotonInstalled()
		{
			return GetPhotonAssemblies().Count > 0;
		}

		private List<Assembly> GetPhotonAssemblies()
		{
			List<Assembly> assemblies = CompilationPipeline.GetAssemblies().ToList();
			return assemblies.FindAll(a => a.name.Equals(PhotonRealTimeAssembly) || a.name.Equals(PhotonUnityAssembly));
		}

		private void UpdatePhotonModuleReferences()
		{
			if (isLocked)
			{
				return;
			}

			string photonModulePath = AssetDatabase.GUIDToAssetPath(PhotonModuleGUID);
			if (string.IsNullOrEmpty(photonModulePath))
			{
				Debug.LogError("Could not load the Impossible Odds Photon assembly definition file.");
				return;
			}

			AssemblyDefinition photonModuleAsmDef = JsonUtility.FromJson<AssemblyDefinition>(File.ReadAllText(photonModulePath));
			List<Assembly> photonAssemblies = GetPhotonAssemblies();

			foreach (Assembly photonAssembly in photonAssemblies)
			{
				if (isSet && !photonModuleAsmDef.references.Contains(photonAssembly.name))
				{
					photonModuleAsmDef.references.Add(photonAssembly.name);
				}
				else if (!isSet && photonModuleAsmDef.references.Contains(photonAssembly.name))
				{
					photonModuleAsmDef.references.Remove(photonAssembly.name);
				}
			}

			File.WriteAllText(photonModulePath, JsonUtility.ToJson(photonModuleAsmDef, true));
			AssetDatabase.Refresh();
		}
	}
}
