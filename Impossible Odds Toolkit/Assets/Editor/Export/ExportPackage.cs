using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ImpossibleOdds.Settings;

public static class ExportPackage
{
	private const string ExportPackageDirectoryKey = "ImpossibleOdds_Export_Path";
	private const string ExportPackageNameKey = "ImpossibleOdds_Export_Filename";

	private const string PackageExtension = "unitypackage";

	[MenuItem("Assets/Impossible Odds/Export")]
	private static void Export()
	{
		string path = EditorPrefs.GetString(ExportPackageDirectoryKey, Application.dataPath);
		string name = EditorPrefs.GetString(ExportPackageNameKey, "Impossible Odds Toolkit");
		string fullPath = EditorUtility.SaveFilePanel("Export Impossible Odds Package", path, name, PackageExtension);

		if (string.IsNullOrEmpty(fullPath))
		{
			return;
		}

		FileInfo fileInfo = new FileInfo(fullPath);
		path = fileInfo.DirectoryName;
		name = Path.GetFileNameWithoutExtension(fileInfo.Name);
		EditorPrefs.SetString(ExportPackageDirectoryKey, path);
		EditorPrefs.SetString(ExportPackageNameKey, name);

		HashSet<string> loadedSymbols = ProjectSettings.GetProjectSymbols(EditorUserBuildSettings.selectedBuildTargetGroup);
		PhotonSettings photonSetting = new PhotonSettings(loadedSymbols);
		bool isSet = photonSetting.IsSet;

		if (isSet)
		{
			photonSetting.IsSet = false;
			photonSetting.ApplyChanges();
			ApplyLoadedSymbols(loadedSymbols, EditorUserBuildSettings.selectedBuildTargetGroup);
		}

		AssetDatabase.ExportPackage("Assets/Impossible Odds", fullPath, ExportPackageOptions.Recurse);

		if (isSet)
		{
			photonSetting.IsSet = true;
			photonSetting.ApplyChanges();
			ApplyLoadedSymbols(loadedSymbols, EditorUserBuildSettings.selectedBuildTargetGroup);
		}
	}

	private static void ApplyLoadedSymbols(HashSet<string> symbols, BuildTargetGroup targetGroup)
	{
		string symbolStr = string.Join(";", symbols.ToArray());
		PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, symbolStr);
	}
}
