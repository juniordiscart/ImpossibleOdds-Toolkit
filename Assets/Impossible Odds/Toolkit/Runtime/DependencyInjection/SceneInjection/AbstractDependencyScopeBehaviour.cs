using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ImpossibleOdds.DependencyInjection
{
	public abstract class AbstractDependencyScopeBehaviour : MonoBehaviour, IDependencyScope
	{
		[SerializeField, Tooltip("Perform the injection process on Start.")]
		private bool injectOnStart = true;

		public IDependencyContainer DependencyContainer { get; protected set; } = null;

		public abstract void Inject();

		protected virtual void Awake()
		{
			InstallContainer();

			if (DependencyContainer != null)
			{
				InstallBindings();
			}
		}

		protected virtual void Start()
		{
			if (injectOnStart && (DependencyContainer != null))
			{
				Inject();
			}
		}

		protected virtual void InstallContainer()
		{
			IEnumerable<IDependencyContainerProvider> providers = GetComponents<IDependencyContainerProvider>();
			if (providers.Any())
			{
				providers = providers.OrderByDescending(p => p.Priority);
				IDependencyContainerProvider provider = providers.First();
				DependencyContainer = provider.GetContainer();
			}
			else
			{
				DependencyContainer = new DependencyContainer();
			}
		}

		protected virtual void InstallBindings()
		{
			IDependencyScopeInstaller[] installers = GetComponentsInChildren<IDependencyScopeInstaller>(false);
			foreach (IDependencyScopeInstaller installer in installers)
			{
				installer.Install(DependencyContainer);
			}
		}
	}
}