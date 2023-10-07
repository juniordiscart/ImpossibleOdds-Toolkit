using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ImpossibleOdds.Addressables
{
	/// <summary>
	/// A loading handle specifically for loading GameObjects.
	/// </summary>
	public class GameObjectLoadingHandle : AssetLoadingHandle<GameObject>
	{
		/// <summary>
		/// The loading handle.
		/// </summary>
		public new AsyncOperationHandle<GameObject> LoadingHandle => loadingHandle;

		/// <summary>
		/// The loaded GameObject when the loading operation succeeds.
		/// </summary>
		public GameObject GameObject => IsSuccess ? loadingHandle.Result : null;

		public GameObjectLoadingHandle(string assetAddress)
		: base(assetAddress)
		{ }

		public GameObjectLoadingHandle(AsyncOperationHandle<GameObject> loadingHandle)
		: base(loadingHandle)
		{ }

		public GameObjectLoadingHandle(AssetReferenceGameObject gameObjectReference)
		: base(gameObjectReference)
		{ }
	}
}