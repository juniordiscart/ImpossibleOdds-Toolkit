namespace ImpossibleOdds.Addressables
{
	using UnityEngine;
	using UnityEngine.ResourceManagement.AsyncOperations;

	/// <summary>
	/// A loading handle to retrieve a GameObject with a component of a specific type on it.
	/// </summary>
	/// <typeparam name="T">The component type on the </typeparam>
	public class MonoBehaviourLoadingHandle<T> : GameObjectLoadingHandle
	where T : MonoBehaviour
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

		public MonoBehaviourLoadingHandle(string assetAddress)
		: base(assetAddress)
		{ }

		public MonoBehaviourLoadingHandle(AsyncOperationHandle<GameObject> loadingHandle)
		: base(loadingHandle)
		{ }

		public override void Dispose()
		{
			componentCache = null;
			base.Dispose();
		}
	}
}
