namespace ImpossibleOdds.Addressables
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.AddressableAssets;
	using UnityEngine.ResourceManagement.AsyncOperations;
	using UnityEngine.ResourceManagement.ResourceLocations;

	/// <summary>
	/// A loading handle to load in multiple game objects.
	/// </summary>
	public class MultiGameObjectLoadingHandle : MultiAssetLoadingHandle<GameObject>
	{
		/// <summary>
		/// The result of the loading operation.
		/// </summary>
		public IList<GameObject> GameObjects
		{
			get { return IsSuccess ? assetsLoadingHandle.Result : null; }
		}

		public MultiGameObjectLoadingHandle(AsyncOperationHandle<IList<IResourceLocation>> locationLoadingHandle)
		: base(locationLoadingHandle)
		{ }

		public MultiGameObjectLoadingHandle(IList<string> keys)
		: base(keys)
		{ }

		public MultiGameObjectLoadingHandle(IList<string> keys, Addressables.MergeMode mergeMode)
		: base(keys, mergeMode)
		{ }
	}
}
