namespace ImpossibleOdds.Addressables
{
	using System;
	using UnityEngine;

	public interface IComponentLoadingHandle<TComponent> : IAddressablesLoadingHandle<GameObject>
	where TComponent : Component
	{
		new event Action<IComponentLoadingHandle<TComponent>> onCompleted;

		TComponent Component
		{
			get;
		}

		new TComponent WaitForCompletion();
	}
}
