using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ImpossibleOdds.Addressables
{
	using Addressables = UnityEngine.AddressableAssets.Addressables;

	/// <summary>
	/// Generic loading handle.
	/// </summary>
	/// <typeparam name="TObject">Type of asset that should be expected to load by the loading handle.</typeparam>
	public class GenericLoadingHandle<TObject> : IAddressablesLoadingHandle<TObject>
	// where TObject : UnityEngine.Object
	{
		protected readonly AsyncOperationHandle<TObject> loadingHandle;

		/// <inheritdoc />
		public event Action<IAddressablesLoadingHandle<TObject>> onCompleted;

		/// <inheritdoc />
		event Action<IAddressablesLoadingHandle> IAddressablesLoadingHandle.onCompleted
		{
			add => onCompleted += value;
			remove => onCompleted -= value;
		}

		/// <inheritdoc />
		public AsyncOperationHandle<TObject> LoadingHandle => loadingHandle;

		/// <inheritdoc />
		public float Progress => loadingHandle.IsValid() ? loadingHandle.PercentComplete : 0f;

		/// <inheritdoc />
		public bool IsDone => loadingHandle.IsValid() && loadingHandle.IsDone;

		/// <inheritdoc />
		public bool IsSuccess => IsDone && (loadingHandle.Status == AsyncOperationStatus.Succeeded);

		/// <inheritdoc />
		public bool IsDisposed { get; private set; }

		/// <inheritdoc />
		public TObject Result => loadingHandle.Result;

		/// <inheritdoc />
		public Task<TObject> Task => loadingHandle.Task;

		/// <inheritdoc />
		object IAddressablesLoadingHandle.Result => Result;

		/// <inheritdoc />
		object IEnumerator.Current => null;

		/// <inheritdoc />
		Task IAddressablesLoadingHandle.Task => Task;

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
			if (IsDisposed || !loadingHandle.IsValid())
			{
				return;
			}

			if (IsDone)
			{
				Addressables.Release(loadingHandle);
			}

			IsDisposed = true;
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
			// If the handle is already disposed off, then unload the handle immediately.
			if (IsDisposed)
			{
				Addressables.Release(loadingHandle);
				return;
			}

			onCompleted.InvokeIfNotNull(this);
		}
	}
}