namespace ImpossibleOdds.DependencyInjection
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public abstract class AbstractDependencyContextBehaviour : MonoBehaviour, IDependencyContext
	{
		[SerializeField, Tooltip("Perform the injection process on Start.")]
		private bool injectOnStart = true;

		private IDependencyContainer container = null;

		public IDependencyContainer DependencyContainer
		{
			get { return container; }
			protected set { container = value; }
		}

		public abstract void Inject();

		protected virtual void Awake()
		{
			InstallContainer();

			if (container != null)
			{
				InstallBindings();
			}
		}

		protected virtual void Start()
		{
			if (injectOnStart && (container != null))
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
				container = provider.GetContainer();
			}
			else
			{
				container = new DependencyContainer();
			}
		}

		protected virtual void InstallBindings()
		{
			IDependencyContextInstaller[] installers = GetComponentsInChildren<IDependencyContextInstaller>(false);
			foreach (IDependencyContextInstaller installer in installers)
			{
				installer.Install(container);
			}
		}
	}
}
