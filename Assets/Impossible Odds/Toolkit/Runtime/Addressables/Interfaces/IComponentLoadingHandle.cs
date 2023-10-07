using System;
using UnityEngine;

namespace ImpossibleOdds.Addressables
{
	/// <summary>
	/// A loading handle interface for game objects being loaded from Addressables.
	/// Fetches the requested type of component right form the game object upon completion.
	/// </summary>
	/// <typeparam name="TComponent">The component to retrieve from the game object once the is loading complete.</typeparam>
	public interface IComponentLoadingHandle<out TComponent> : IAddressablesLoadingHandle<GameObject>
	where TComponent : Component
	{
		/// <summary>
		/// Invoked when the loading operation has completed.
		/// </summary>
		new event Action<IComponentLoadingHandle<TComponent>> onCompleted;

		/// <summary>
		/// The component fetched from the result.
		/// </summary>
		TComponent Component
		{
			get;
		}

		/// <summary>
		/// Wait for the loading to complete and get the component from the result.
		/// </summary>
		/// <returns>The component from the result.</returns>
		new TComponent WaitForCompletion();
	}
}