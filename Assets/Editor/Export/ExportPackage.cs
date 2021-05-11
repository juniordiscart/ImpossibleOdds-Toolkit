using System.IO;
using UnityEngine;
using UnityEditor;

public static class ExportPackage
{
	private const string ExportPackageDirectoryKey = "ImpossibleOdds_Export_Path";
	private const string ExportPackageNameKey = "ImpossibleOdds_Export_Filename";
	private const string ExportPackageExamplesNameKey = "ImpossibleOdds_Export_Examples_Filename";

	private const string PackageExtension = "unitypackage";

	[MenuItem("Assets/Impossible Odds/Export Toolkit")]
	private static void ExportToolkit()
	{
		string path = EditorPrefs.GetString(ExportPackageDirectoryKey, Application.dataPath);
		string name = EditorPrefs.GetString(ExportPackageNameKey, "Impossible Odds Toolkit");
		string fullPath = EditorUtility.SaveFilePanel("Export Impossible Odds Toolkit", path, name, PackageExtension);

		if (string.IsNullOrEmpty(fullPath))
		{
			return;
		}

		FileInfo fileInfo = new FileInfo(fullPath);
		path = fileInfo.DirectoryName;
		name = Path.GetFileNameWithoutExtension(fileInfo.Name);
		EditorPrefs.SetString(ExportPackageDirectoryKey, path);
		EditorPrefs.SetString(ExportPackageNameKey, name);

		AssetDatabase.ExportPackage("Assets/Impossible Odds", fullPath, ExportPackageOptions.Recurse);
	}

	// [MenuItem("Assets/Impossible Odds/Export Toolkit Examples")]
	// private static void ExportExamples()
	// {
	// 	string path = EditorPrefs.GetString(ExportPackageDirectoryKey, Application.dataPath);
	// 	string name = EditorPrefs.GetString(ExportPackageExamplesNameKey, "Impossible Odds Toolkit Examples");
	// 	string fullPath = EditorUtility.SaveFilePanel("Export Impossible Odds Examples", path, name, PackageExtension);

	// 	if (string.IsNullOrEmpty(fullPath))
	// 	{
	// 		return;
	// 	}

	// 	FileInfo fileInfo = new FileInfo(fullPath);
	// 	path = fileInfo.DirectoryName;
	// 	name = Path.GetFileNameWithoutExtension(fileInfo.Name);
	// 	EditorPrefs.SetString(ExportPackageDirectoryKey, path);
	// 	EditorPrefs.SetString(ExportPackageExamplesNameKey, name);

	// 	AssetDatabase.ExportPackage("Assets/Examples/Impossible Odds", fullPath, ExportPackageOptions.Recurse);
	// }
}
