namespace ImpossibleOdds.Addressables
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.AddressableAssets;
	using UnityEngine.ResourceManagement.AsyncOperations;
	using UnityEngine.ResourceManagement.ResourceLocations;

	/// <summary>
	/// A loading handle for loading in multiple GameObjects with a specific type of component on it.
	/// </summary>
	/// <typeparam name="T">The type of the component to load.</typeparam>
	public class MultiComponentLoadingHandle<T> : MultiGameObjectLoadingHandle
	where T : Component
	{
		private IList<T> components = null;

		public virtual IList<T> Components
		{
			get
			{
				if (components != null)
				{
					return components;
				}

				if (IsSuccess)
				{
					components = new List<T>(GameObjects.Count);
					foreach (GameObject obj in GameObjects)
					{
						T component = obj.GetComponent<T>();
						if (component != null)
						{
							components.Add(component);
						}
					}
				}

				return components;
			}
		}

		public MultiComponentLoadingHandle(AsyncOperationHandle<IList<IResourceLocation>> locationLoadingHandle)
		: base(locationLoadingHandle)
		{ }

		public MultiComponentLoadingHandle(IList<string> keys)
		: base(keys)
		{ }

		public MultiComponentLoadingHandle(IList<string> keys, Addressables.MergeMode mergeMode)
		: base(keys, mergeMode)
		{ }
	}

}
