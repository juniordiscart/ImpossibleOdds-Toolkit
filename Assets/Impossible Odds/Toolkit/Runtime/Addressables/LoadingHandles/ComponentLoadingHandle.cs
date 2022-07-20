namespace ImpossibleOdds.Addressables
{
	using System;
	using UnityEngine;
	using UnityEngine.AddressableAssets;
	using UnityEngine.ResourceManagement.AsyncOperations;

	/// <summary>
	/// A loading handle to retrieve a GameObject with a component of a specific type on it.
	/// </summary>
	/// <typeparam name="T">The component type on the </typeparam>
	public class ComponentLoadingHandle<T> : GameObjectLoadingHandle, IComponentLoadingHandle<T>
	where T : Component
	{
		/// <inheritdoc />
		public new event Action<IComponentLoadingHandle<T>> onCompleted;

		private T componentCache = null;

		/// <inheritdoc />
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
		{
			base.onCompleted += OnCompleted;
		}

		public ComponentLoadingHandle(AsyncOperationHandle<GameObject> loadingHandle)
		: base(loadingHandle)
		{
			base.onCompleted += OnCompleted;
		}

		public ComponentLoadingHandle(AssetReferenceGameObject gameObjectReference)
		: base(gameObjectReference)
		{
			base.onCompleted += OnCompleted;
		}

		/// <inheritdoc />
		public new T WaitForCompletion()
		{
			base.WaitForCompletion();
			return Component;
		}

		/// <inheritdoc />
		public override void Dispose()
		{
			componentCache = null;
			base.Dispose();
		}

		private void OnCompleted(AssetLoadingHandle<GameObject> handle)
		{
			onCompleted.InvokeIfNotNull(this);
		}
	}
}
