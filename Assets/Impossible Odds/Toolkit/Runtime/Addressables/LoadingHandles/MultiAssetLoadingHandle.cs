namespace ImpossibleOdds.Addressables
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.AddressableAssets;
	using UnityEngine.ResourceManagement.AsyncOperations;
	using UnityEngine.ResourceManagement.ResourceLocations;
	using ImpossibleOdds;
	using System.Threading.Tasks;

	public class MultiAssetLoadingHandle<TObject> : IMultiAddressablesLoadingHandle<TObject>
	where TObject : UnityEngine.Object
	{
		protected AsyncOperationHandle<IList<IResourceLocation>> locationLoadingHandle;
		protected AsyncOperationHandle<IList<TObject>> assetsLoadingHandle;
		private bool isDisposed = false;
		private bool isDone = false;

		public event Action<IMultiAddressablesLoadingHandle<TObject>> onCompleted;

		public AsyncOperationHandle<IList<IResourceLocation>> ResourceLocationLoadingHandle
		{
			get { return locationLoadingHandle; }
		}

		public AsyncOperationHandle<IList<TObject>> AssetsLoadingHandle
		{
			get { return assetsLoadingHandle; }
		}

		public bool IsDone
		{
			get { return isDone; }
		}

		public float Progress
		{
			get { return (locationLoadingHandle.IsDone && assetsLoadingHandle.IsValid()) ? assetsLoadingHandle.PercentComplete : 0f; }
		}

		public bool IsSuccess
		{
			get
			{
				return
					IsDone &&
					(assetsLoadingHandle.Status == AsyncOperationStatus.Succeeded) &&
					(locationLoadingHandle.Status == AsyncOperationStatus.Succeeded);
			}
		}

		public bool IsDisposed
		{
			get => isDisposed;
		}

		public IList<TObject> Result
		{
			get => assetsLoadingHandle.Result;
		}

		public Task<IList<TObject>> Task
		{
			get => assetsLoadingHandle.Task;
		}

		object IAddressablesLoadingHandle.Result
		{
			get => Result;
		}

		object IEnumerator.Current
		{
			get => null;
		}

		Task IAddressablesLoadingHandle.Task
		{
			get => assetsLoadingHandle.Task;
		}

		event Action<IAddressablesLoadingHandle> IAddressablesLoadingHandle.onCompleted
		{
			add { onCompleted += value; }
			remove { onCompleted -= value; }
		}

		public MultiAssetLoadingHandle(IEnumerable keys)
		: this(keys, Addressables.MergeMode.Union)
		{ }

		public MultiAssetLoadingHandle(IEnumerable keys, Addressables.MergeMode mergeMode)
		{
			this.locationLoadingHandle = Addressables.LoadResourceLocationsAsync(keys, mergeMode, typeof(TObject));
			locationLoadingHandle.Completed += OnLocationLoadingCompleted;
		}

		public MultiAssetLoadingHandle(AsyncOperationHandle<IList<IResourceLocation>> locationLoadingHandle)
		{
			if (!locationLoadingHandle.IsValid())
			{
				throw new ArgumentException("The handle is not valid.");
			}

			this.locationLoadingHandle = locationLoadingHandle;
			locationLoadingHandle.Completed += OnLocationLoadingCompleted;
		}

		public virtual void Dispose()
		{
			if (isDisposed)
			{
				return;
			}

			if (assetsLoadingHandle.IsValid())
			{
				Addressables.Release(assetsLoadingHandle);
			}

			if (locationLoadingHandle.IsValid())
			{
				Addressables.Release(locationLoadingHandle);
			}

			isDone = true;
			isDisposed = true;
		}

		/// <inheritdoc />
		public IList<TObject> WaitForCompletion()
		{
			if (locationLoadingHandle.IsValid() && !locationLoadingHandle.IsDone)
			{
				locationLoadingHandle.WaitForCompletion();
				locationLoadingHandle.Completed -= OnLocationLoadingCompleted;
			}

			if (!assetsLoadingHandle.IsValid() && locationLoadingHandle.IsValid() && locationLoadingHandle.IsDone)
			{
				OnLocationLoadingCompleted(locationLoadingHandle);
			}

			if (assetsLoadingHandle.IsValid() && !assetsLoadingHandle.IsDone)
			{
				assetsLoadingHandle.WaitForCompletion();
				assetsLoadingHandle.Completed -= OnAssetsLoadingCompleted;
				OnAssetsLoadingCompleted(assetsLoadingHandle);
			}

			return assetsLoadingHandle.Result;
		}

		private void OnLocationLoadingCompleted(AsyncOperationHandle<IList<IResourceLocation>> r)
		{
			if (locationLoadingHandle.Status != AsyncOperationStatus.Succeeded)
			{
				Debug.LogErrorFormat("The loading of resource locations in loadinghandle of type '{0}' did not complete successfully.", this.GetType().Name);
				isDone = true;
				return;
			}

			assetsLoadingHandle = Addressables.LoadAssetsAsync<TObject>(r.Result, null);
			assetsLoadingHandle.Completed += OnAssetsLoadingCompleted;
		}

		private void OnAssetsLoadingCompleted(AsyncOperationHandle<IList<TObject>> r)
		{
			isDone = true;
			onCompleted.InvokeIfNotNull(this);
		}

		/// <inheritdoc />
		bool IEnumerator.MoveNext()
		{
			return !IsDone;
		}

		/// <inheritdoc />
		void IEnumerator.Reset()
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		object IAddressablesLoadingHandle.WaitForCompletion()
		{
			return WaitForCompletion();
		}
	}
}
