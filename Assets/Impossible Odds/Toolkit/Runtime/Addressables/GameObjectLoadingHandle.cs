namespace ImpossibleOdds.Addressables
{
	using UnityEngine;
	using UnityEngine.ResourceManagement.AsyncOperations;

	/// <summary>
	/// A loading handle specifically for loading GameObjects.
	/// </summary>
	public class GameObjectLoadingHandle : AssetLoadingHandle<GameObject>
	{
		/// <summary>
		/// The loading handle.
		/// </summary>
		public new AsyncOperationHandle<GameObject> LoadingHandle
		{
			get { return loadingHandle; }
		}

		/// <summary>
		/// The loaded GameObject when the loading operation succeeds.
		/// </summary>
		public GameObject GameObject
		{
			get { return IsSuccess ? loadingHandle.Result : null; }
		}

		public GameObjectLoadingHandle(string assetAddress)
		: base(assetAddress)
		{ }

		public GameObjectLoadingHandle(AsyncOperationHandle<GameObject> loadingHandle)
		: base(loadingHandle)
		{ }
	}
}
