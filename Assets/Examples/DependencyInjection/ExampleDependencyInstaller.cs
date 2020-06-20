namespace ImpossibleOdds.Testing.DependencyInjection
{
	using UnityEngine;
	using ImpossibleOdds.DependencyInjection;

	public class ExampleDependencyInstaller : MonoBehaviour, IDependencyContextInstaller
	{
		[SerializeField]
		private Camera characterFollowCamera = null;
		[SerializeField]
		private CharacterSettings settings = null;

		public void Install(IDependencyContext context)
		{
			// Create an input manager and bind its implemented interfaces
			KeyboardInputManager keyboard = new KeyboardInputManager();
			context.DependencyContainer.BindWithInterfaces<KeyboardInputManager>(new InstanceBinding<KeyboardInputManager>(keyboard));

			if (settings != null)
			{
				context.DependencyContainer.Bind(new InstanceBinding<CharacterSettings>(settings));
			}

			if (characterFollowCamera != null)
			{
				context.DependencyContainer.Bind(new InstanceBinding<Camera>(characterFollowCamera));
			}
		}
	}
}
