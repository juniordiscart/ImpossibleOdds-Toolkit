namespace ImpossibleOdds.Addressables
{
	using UnityEngine;
	using UnityEngine.AddressableAssets;
	using UnityEngine.ResourceManagement.AsyncOperations;

	/// <summary>
	/// A loading handle to retrieve a GameObject with a component of a specific type on it.
	/// </summary>
	/// <typeparam name="T">The component type on the </typeparam>
	public class ComponentLoadingHandle<T> : GameObjectLoadingHandle
	where T : Component
	{
		private T componentCache = null;

		public virtual T Component
		{
			get
			{
				if (componentCache != null)
				{
					return componentCache;
				}

				if (IsSuccess)
				{
					componentCache = GameObject.GetComponent<T>();
				}

				return componentCache;
			}
		}

		public ComponentLoadingHandle(string assetAddress)
		: base(assetAddress)
		{ }

		public ComponentLoadingHandle(AsyncOperationHandle<GameObject> loadingHandle)
		: base(loadingHandle)
		{ }

		public ComponentLoadingHandle(AssetReferenceGameObject gameObjectReference)
		: base(gameObjectReference)
		{ }

		public override void Dispose()
		{
			componentCache = null;
			base.Dispose();
		}
	}
}
