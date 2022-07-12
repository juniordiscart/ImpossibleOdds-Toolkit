namespace ImpossibleOdds.Addressables
{
	using System;
	using UnityEngine.AddressableAssets;
	using UnityEngine.ResourceManagement.ResourceLocations;
	using UnityEngine.ResourceManagement.AsyncOperations;
	using ImpossibleOdds;

	/// <summary>
	/// Generic loading handle to load in Unity-specific objects.
	/// </summary>
	/// <typeparam name="T">Unity-specific Object type.</typeparam>
	public class AssetLoadingHandle<T> : GenericLoadingHandle<T>
	where T : UnityEngine.Object
	{
		/// <inheritdoc />
		public new event Action<AssetLoadingHandle<T>> onCompleted;

		/// <summary>
		/// Load the object using it's asset address.
		/// </summary>
		/// <param name="assetAddress">The address of the asset.</param>
		public AssetLoadingHandle(string assetAddress)
		: base(Addressables.LoadAssetAsync<T>(assetAddress))
		{
			base.onCompleted += OnCompleted;
		}

		/// <summary>
		/// Load the object using an asset reference.
		/// </summary>
		/// <param name="reference">The asset reference.</param>
		public AssetLoadingHandle(AssetReferenceT<T> reference)
		: base(reference.LoadAssetAsync())
		{
			base.onCompleted += OnCompleted;
		}

		/// <summary>
		/// Load the object using it's resource location.
		/// </summary>
		/// <param name="location">The resource location of the asset.</param>
		public AssetLoadingHandle(IResourceLocation location)
		: base(Addressables.LoadAssetAsync<T>(location))
		{
			base.onCompleted += OnCompleted;
		}

		/// <summary>
		/// Hook this loading handle into an existing/ongoing loading.
		/// </summary>
		/// <param name="loadingHandle">The loading handle to hook into.</param>
		public AssetLoadingHandle(AsyncOperationHandle<T> loadingHandle)
		: base(loadingHandle)
		{
			base.onCompleted += OnCompleted;
		}

		private void OnCompleted(IAddressablesLoadingHandle<T> handle)
		{
			onCompleted.InvokeIfNotNull(this);
		}
	}
}
