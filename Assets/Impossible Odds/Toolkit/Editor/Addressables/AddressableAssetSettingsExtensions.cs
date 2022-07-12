namespace ImpossibleOdds.Editor.Addressables
{
	using UnityEditor;
	using UnityEditor.AddressableAssets.Settings;
	using ImpossibleOdds;

	public static class AddressableAssetSettingsExtensions
	{
		/// <summary>
		/// Is the asset registered as an addressable asset with these settings?
		/// </summary>
		/// <param name="s">The settings containing the groups for which to check the asset.</param>
		/// <param name="asset">The asset to check whether it is an addressable asset or not.</param>
		/// <returns>True if the asset is a registered asset, false otherwise.</returns>
		public static bool IsAssetAddressable(this AddressableAssetSettings s, UnityEngine.Object asset)
		{
			return s.FindAssetEntry(asset) != null;
		}

		/// <summary>
		/// Find the addressable asset group to which the asset belongs to, provided it is registered as an addressable asset.
		/// </summary>
		/// <param name="s">The settings containing the groups for which to find the asset group the asset belongs to.</param>
		/// <param name="asset">The asset for which to find the asset group it belongs to.</param>
		/// <returns>The group in the settings, null otherwise.</returns>
		public static AddressableAssetGroup FindAssetGroup(this AddressableAssetSettings s, UnityEngine.Object asset)
		{
			AddressableAssetEntry entry = s.FindAssetEntry(asset);
			return (entry != null) ? entry.parentGroup : null;
		}

		/// <summary>
		/// Find the addressable asset entry for the given asset in these settings.
		/// </summary>
		/// <param name="s">The settings containing the groups for which to fetch the asset entry.</param>
		/// <param name="asset">The asset for which to find the addressable asset entry in the settings.</param>
		/// <returns>The entry in the settings, null otherwise.</returns>
		public static AddressableAssetEntry FindAssetEntry(this AddressableAssetSettings s, UnityEngine.Object asset)
		{
			s.ThrowIfNull(nameof(s));
			asset.ThrowIfNull(nameof(asset));
			string assetGuid = GetAssetGuid(asset);

			if (string.IsNullOrWhiteSpace(assetGuid))
			{
				return null;
			}

			return s.FindAssetEntry(assetGuid);
		}

		/// <summary>
		/// Fetches the guid of the asset if it is registered in the AssetDatabase.
		/// </summary>
		/// <param name="asset">The asset for which to fetch the guid.</param>
		/// <returns>The asset's guid if it is found in the asset database, an empty string otherwise.</returns>
		public static string GetAssetGuid(UnityEngine.Object asset)
		{
			asset.ThrowIfNull(nameof(asset));
			return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset));
		}
	}
}
