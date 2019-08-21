namespace ImpossibleOdds.DependencyInjection
{
	using System;
	using UnityEngine;
	using UnityEngine.SceneManagement;

	using ImpossibleOdds.DependencyInjection.Extensions;

	[ScriptExecutionOrder(-9998)]
	public class SceneDependencyContext : MonoBehaviour, IDependencyContext
	{
		public IDependencyContainer DependencyContainer
		{
			get { return sceneContainer; }
		}

		[SerializeField, Tooltip("When ticked, the scene will get injected on load using the context installers found on this scene context.")]
		private bool injectSceneOnLoad = true;
		[SerializeField, Tooltip("When ticked, injection will also be performed using the global registered context.")]
		private bool injectWithGlobalContext = true;
		[SerializeField, Tooltip("Unique identifier that is used to identify this context in the dependency register.")]
		private string contextID = Guid.NewGuid().ToString();

		private DependencyContainer sceneContainer = new DependencyContainer();

		private void Awake()
		{
			InstallBindings();
			DependencyContextRegister.Register(contextID, this);

			if (!injectSceneOnLoad)
			{
				return;
			}

			// Inject with the global context first.
			if (injectWithGlobalContext && DependencyContextRegister.Exists(GlobalDependencyContext.GlobalID))
			{
				InjectScene(DependencyContextRegister.GetContext(GlobalDependencyContext.GlobalID));
			}

			InjectScene(this);
		}

		private void OnDestroy()
		{
			if (DependencyContextRegister.Exists(contextID) && DependencyContextRegister.GetContext(contextID).Equals(this))
			{
				DependencyContextRegister.Remove(contextID);
			}
		}

		private void InstallBindings()
		{
			IDependencyContextInstaller[] installers = GetComponentsInChildren<IDependencyContextInstaller>(false);
			foreach (IDependencyContextInstaller installer in installers)
			{
				installer.Install(this);
			}
		}

		private void InjectScene(IDependencyContext context)
		{
			Scene currentScene = gameObject.scene;
			currentScene.GetRootGameObjects().Inject(context, true);
		}
	}
}
