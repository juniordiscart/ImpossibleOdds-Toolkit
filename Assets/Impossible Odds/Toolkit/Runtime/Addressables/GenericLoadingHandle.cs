namespace ImpossibleOdds.Addressables
{
	using System;
	using System.Collections;
	using UnityEngine.AddressableAssets;
	using UnityEngine.ResourceManagement.AsyncOperations;
	using ImpossibleOdds;

	/// <summary>
	/// Generic loading handle.
	/// </summary>
	/// <typeparam name="T">Type of asset that should be expected to load by the loading handle.</typeparam>
	public class GenericLoadingHandle<T> : IAddressablesLoadingHandle<T>
	{
		protected readonly AsyncOperationHandle<T> loadingHandle;
		private bool disposed = false;

		/// <inheritdoc />
		public event Action<IAddressablesLoadingHandle<T>> onCompleted;

		/// <inheritdoc />
		event Action<IAddressablesLoadingHandle> IAddressablesLoadingHandle.onCompleted
		{
			add { onCompleted += value; }
			remove { onCompleted -= value; }
		}

		/// <inheritdoc />
		public AsyncOperationHandle<T> LoadingHandle
		{
			get { return loadingHandle; }
		}

		/// <inheritdoc />
		public float Progress
		{
			get { return loadingHandle.PercentComplete; }
		}

		/// <inheritdoc />
		public bool IsDone
		{
			get { return loadingHandle.IsDone; }
		}

		/// <inheritdoc />
		public bool IsSuccess
		{
			get { return IsDone && (loadingHandle.Status == AsyncOperationStatus.Succeeded); }
		}

		/// <inheritdoc />
		public bool IsDisposed
		{
			get { return disposed; }
		}

		/// <inheritdoc />
		public T Result
		{
			get { return loadingHandle.Result; }
		}

		/// <inheritdoc />
		object IAddressablesLoadingHandle.Result
		{
			get { return Result; }
		}

		object IEnumerator.Current
		{
			get { return null; }
		}

		public GenericLoadingHandle(AsyncOperationHandle<T> loadingHandle)
		{
			if (!loadingHandle.IsValid())
			{
				throw new ArgumentException("The handle is not valid.");
			}

			this.loadingHandle = loadingHandle;
			this.loadingHandle.Completed += OnCompleted;
		}

		/// <summary>
		/// Releases the underlying asset loading handle.
		/// </summary>
		public virtual void Dispose()
		{
			if (disposed || !loadingHandle.IsValid())
			{
				return;
			}

			Addressables.Release(loadingHandle);
			disposed = true;
		}

		/// <inheritdoc />
		public virtual void WaitForCompletion()
		{
			// If the handle is already done, then don't complete again.
			if (!IsDone)
			{
				loadingHandle.WaitForCompletion();
			}
		}

		bool IEnumerator.MoveNext()
		{
			return !IsDone;
		}

		void IEnumerator.Reset()
		{
			throw new NotImplementedException();
		}

		private void OnCompleted(AsyncOperationHandle<T> handle)
		{
			onCompleted.InvokeIfNotNull(this);
		}
	}
}
