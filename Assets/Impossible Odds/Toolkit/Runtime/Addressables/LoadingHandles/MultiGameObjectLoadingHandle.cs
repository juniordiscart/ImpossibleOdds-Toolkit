using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace ImpossibleOdds.Addressables
{
	using Addressables = UnityEngine.AddressableAssets.Addressables;

	/// <summary>
	/// A loading handle to load in multiple game objects.
	/// </summary>
	public class MultiGameObjectLoadingHandle : MultiAssetLoadingHandle<GameObject>
	{
		/// <summary>
		/// The result of the loading operation.
		/// </summary>
		public IList<GameObject> GameObjects => IsSuccess ? assetsLoadingHandle.Result : null;

		public MultiGameObjectLoadingHandle(AsyncOperationHandle<IList<IResourceLocation>> locationLoadingHandle)
		: base(locationLoadingHandle)
		{ }

		public MultiGameObjectLoadingHandle(IEnumerable<string> keys)
		: base(keys)
		{ }

		public MultiGameObjectLoadingHandle(IEnumerable<string> keys, Addressables.MergeMode mergeMode)
		: base(keys, mergeMode)
		{ }
	}
}