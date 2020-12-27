namespace ImpossibleOdds.Examples.DependencyInjection
{
	using UnityEngine;
	using ImpossibleOdds.DependencyInjection;

	public class ExampleDependencyInstaller : MonoBehaviour, IDependencyContextInstaller
	{
		[SerializeField]
		private CharacterSettings settings = null;

		public void Install(IDependencyContainer container)
		{
			// Create an input manager and bind its implemented interfaces
			KeyboardInputManager keyboard = new KeyboardInputManager();
			container.BindWithInterfaces<KeyboardInputManager>(new InstanceBinding<KeyboardInputManager>(keyboard));

			if (settings != null)
			{
				container.Bind(new InstanceBinding<CharacterSettings>(settings));
			}
		}
	}
}
