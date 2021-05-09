namespace ImpossibleOdds.Examples.DependencyInjection
{
	using UnityEngine;
	using ImpossibleOdds.DependencyInjection;

	public class ExampleDependencyInstaller : MonoBehaviour, IDependencyScopeInstaller
	{
		[SerializeField]
		private CharacterSettings settings = null;

		public void Install(IDependencyContainer container)
		{
			// Create an input manager and bind its implemented interfaces
			KeyboardInputManager keyboard = new KeyboardInputManager();
			container.RegisterWithInterfaces<KeyboardInputManager>(new InstanceBinding<KeyboardInputManager>(keyboard));

			if (settings != null)
			{
				container.Register(new InstanceBinding<CharacterSettings>(settings));
			}
		}
	}
}
