using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using System.Threading.Tasks;

namespace ImpossibleOdds.Addressables
{
	using Addressables = UnityEngine.AddressableAssets.Addressables;

	public class MultiAssetLoadingHandle<TObject> : IMultiAddressablesLoadingHandle<TObject>
	where TObject : UnityEngine.Object
	{
		protected AsyncOperationHandle<IList<IResourceLocation>> locationLoadingHandle;
		protected AsyncOperationHandle<IList<TObject>> assetsLoadingHandle;

		public event Action<IMultiAddressablesLoadingHandle<TObject>> onCompleted;

		public AsyncOperationHandle<IList<IResourceLocation>> ResourceLocationLoadingHandle => locationLoadingHandle;

		public AsyncOperationHandle<IList<TObject>> AssetsLoadingHandle => assetsLoadingHandle;

		public bool IsDone { get; private set; }

		public float Progress => (locationLoadingHandle.IsDone && assetsLoadingHandle.IsValid()) ? assetsLoadingHandle.PercentComplete : 0f;

		public bool IsSuccess =>
			IsDone &&
			(assetsLoadingHandle.Status == AsyncOperationStatus.Succeeded) &&
			(locationLoadingHandle.Status == AsyncOperationStatus.Succeeded);

		public bool IsDisposed { get; private set; }

		public IList<TObject> Result => assetsLoadingHandle.Result;

		public Task<IList<TObject>> Task => assetsLoadingHandle.Task;

		object IAddressablesLoadingHandle.Result => Result;

		object IEnumerator.Current => null;

		Task IAddressablesLoadingHandle.Task => assetsLoadingHandle.Task;

		event Action<IAddressablesLoadingHandle> IAddressablesLoadingHandle.onCompleted
		{
			add => onCompleted += value;
			remove => onCompleted -= value;
		}

		public MultiAssetLoadingHandle(IEnumerable keys, Addressables.MergeMode mergeMode = Addressables.MergeMode.Union)
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
			if (IsDisposed)
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

			IsDone = true;
			IsDisposed = true;
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
				Debug.LogErrorFormat("The loading of resource locations in loading handle of type '{0}' did not complete successfully.", this.GetType().Name);
				IsDone = true;
				return;
			}

			assetsLoadingHandle = Addressables.LoadAssetsAsync<TObject>(r.Result, null);
			assetsLoadingHandle.Completed += OnAssetsLoadingCompleted;
		}

		private void OnAssetsLoadingCompleted(AsyncOperationHandle<IList<TObject>> r)
		{
			IsDone = true;
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