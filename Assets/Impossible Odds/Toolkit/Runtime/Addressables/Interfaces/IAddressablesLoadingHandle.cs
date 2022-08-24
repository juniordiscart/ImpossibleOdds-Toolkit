namespace ImpossibleOdds.Addressables
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	/// <summary>
	/// A basic loading handle interface for fetching Addressable Assets.
	/// </summary>
	public interface IAddressablesLoadingHandle : IEnumerator, IDisposable
	{
		/// <summary>
		/// Invoked when the handle has finished loaded.
		/// </summary>
		event Action<IAddressablesLoadingHandle> onCompleted;

		/// <summary>
		/// Is the handle done loading the asset?
		/// </summary>
		bool IsDone
		{
			get;
		}

		/// <summary>
		/// Did the handle succeed in loading the asset?
		/// </summary>
		bool IsSuccess
		{
			get;
		}

		/// <summary>
		/// Is this loading handle disposed of?
		/// </summary>
		bool IsDisposed
		{
			get;
		}

		/// <summary>
		/// The normalized loading progress value.
		/// </summary>
		float Progress
		{
			get;
		}

		/// <summary>
		/// The loaded asset, if the operation succeeded.
		/// </summary>
		object Result
		{
			get;
		}

		/// <summary>
		/// The task for awaiting the loading asynchronously.
		/// </summary>
		Task Task
		{
			get;
		}

		/// <summary>
		/// Synchronously wait for the asset loading to complete.
		/// </summary>
		object WaitForCompletion();
	}

	/// <summary>
	/// A generic loading handle interface to fetch an asset of a certain type.
	/// </summary>
	/// <typeparam name="TObject">The type of asset to fetch.</typeparam>
	public interface IAddressablesLoadingHandle<TObject> : IAddressablesLoadingHandle
	// where TObject : UnityEngine.Object
	{
		/// <summary>
		/// Invoked when the handle has finished loading.
		/// </summary>
		new event Action<IAddressablesLoadingHandle<TObject>> onCompleted;

		/// <summary>
		/// The loaded asset, if the operation succeeded.
		/// </summary>
		new TObject Result
		{
			get;
		}

		/// <summary>
		/// The task for awaiting the loading asynchronously.
		/// </summary>
		new Task<TObject> Task
		{
			get;
		}

		/// <summary>
		/// Synchronously wait for the asset loading to complete.
		/// </summary>
		new TObject WaitForCompletion();
	}

	/// <summary>
	/// A generic loading handle interface to fetch a collection of assets of the same type.
	/// </summary>
	/// <typeparam name="TObject">The type of the assets to fetch.</typeparam>
	public interface IMultiAddressablesLoadingHandle<TObject> : IAddressablesLoadingHandle
	where TObject : UnityEngine.Object
	{
		/// <summary>
		/// Invoked when the handle has finished loading.
		/// </summary>
		new event Action<IMultiAddressablesLoadingHandle<TObject>> onCompleted;

		/// <summary>
		/// The loaded assets, if the operation succeeded.
		/// </summary>
		new IList<TObject> Result
		{
			get;
		}

		/// <summary>
		/// The task for awaiting the loading asynchonously.
		/// </summary>
		new Task<IList<TObject>> Task
		{
			get;
		}

		/// <summary>
		/// Synchronously wait for the asset loading to complete.
		/// </summary>
		new IList<TObject> WaitForCompletion();
	}
}
