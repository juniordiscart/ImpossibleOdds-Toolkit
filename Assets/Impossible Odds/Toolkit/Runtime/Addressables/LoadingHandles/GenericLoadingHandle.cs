namespace ImpossibleOdds.Addressables
{
	using System;
	using System.Collections;
	using UnityEngine.AddressableAssets;
	using UnityEngine.ResourceManagement.AsyncOperations;
	using ImpossibleOdds;
	using System.Threading.Tasks;

	/// <summary>
	/// Generic loading handle.
	/// </summary>
	/// <typeparam name="TObject">Type of asset that should be expected to load by the loading handle.</typeparam>
	public class GenericLoadingHandle<TObject> : IAddressablesLoadingHandle<TObject>
	// where TObject : UnityEngine.Object
	{
		protected readonly AsyncOperationHandle<TObject> loadingHandle;
		private bool disposed = false;

		/// <inheritdoc />
		public event Action<IAddressablesLoadingHandle<TObject>> onCompleted;

		/// <inheritdoc />
		event Action<IAddressablesLoadingHandle> IAddressablesLoadingHandle.onCompleted
		{
			add => onCompleted += value;
			remove => onCompleted -= value;
		}

		/// <inheritdoc />
		public AsyncOperationHandle<TObject> LoadingHandle
		{
			get => loadingHandle;
		}

		/// <inheritdoc />
		public float Progress
		{
			get => loadingHandle.PercentComplete;
		}

		/// <inheritdoc />
		public bool IsDone
		{
			get => loadingHandle.IsDone;
		}

		/// <inheritdoc />
		public bool IsSuccess
		{
			get => IsDone && (loadingHandle.Status == AsyncOperationStatus.Succeeded);
		}

		/// <inheritdoc />
		public bool IsDisposed
		{
			get => disposed;
		}

		/// <inheritdoc />
		public TObject Result
		{
			get => loadingHandle.Result;
		}

		/// <inheritdoc />
		public Task<TObject> Task
		{
			get => loadingHandle.Task;
		}

		/// <inheritdoc />
		object IAddressablesLoadingHandle.Result
		{
			get => Result;
		}

		/// <inheritdoc />
		object IEnumerator.Current
		{
			get => null;
		}

		/// <inheritdoc />
		Task IAddressablesLoadingHandle.Task
		{
			get => Task;
		}

		public GenericLoadingHandle(AsyncOperationHandle<TObject> loadingHandle)
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
		public virtual TObject WaitForCompletion()
		{
			// If the handle is already done, then don't complete again.
			return !IsDone ? loadingHandle.WaitForCompletion() : loadingHandle.Result;
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

		private void OnCompleted(AsyncOperationHandle<TObject> handle)
		{
			onCompleted.InvokeIfNotNull(this);
		}
	}
}
