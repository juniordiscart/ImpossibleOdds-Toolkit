#if IMPOSSIBLE_ODDS_ENABLE_EDITOR_EXT
namespace ImpossibleOdds.Editor
{
	using UnityEditor;

	public class CopyAssetPathToClipboard : Editor
	{
		[MenuItem("Assets/Copy asset path")]
		private static void GetAssetPath()
		{
			if (Selection.activeObject == null)
			{
				return;
			}

			try
			{
				string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
				EditorGUIUtility.systemCopyBuffer = assetPath;
			}
			catch (System.Exception) { }
		}
	}
}
#endif
