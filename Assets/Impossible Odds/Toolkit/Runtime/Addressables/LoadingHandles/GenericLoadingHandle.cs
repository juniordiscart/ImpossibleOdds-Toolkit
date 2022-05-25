namespace ImpossibleOdds.Addressables
{
	using System;
	using System.Collections;
	using UnityEngine.AddressableAssets;
	using UnityEngine.ResourceManagement.AsyncOperations;
	using ImpossibleOdds;
	using System.Threading.Tasks;
	using System.Runtime.CompilerServices;

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
			add => onCompleted += value;
			remove => onCompleted -= value;
		}

		/// <inheritdoc />
		public AsyncOperationHandle<T> LoadingHandle
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
		public T Result
		{
			get => loadingHandle.Result;
		}

		/// <inheritdoc />
		public Task<T> Task
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
