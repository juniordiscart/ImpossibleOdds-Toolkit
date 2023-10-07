using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ImpossibleOdds.DependencyInjection
{

	/// <summary>
	/// A dependency injection scope that manages resources that are global to the project.
	/// </summary>
	public class GlobalDependencyScope : IDependencyScope
	{
		#region Static
		private static GlobalDependencyScope globalScope = null;

		/// <summary>
		/// The global dependency scope.
		/// </summary>
		public static IDependencyScope GlobalScope => globalScope;

		/// <summary>
		/// Defines whether the global scope should inject its resources when a scene has been loaded.
		/// </summary>
		public static bool AutoInjectLoadedScenes { get; set; }
#if IMPOSSIBLE_ODDS_DEPENDENCY_INJECTION_DISABLE_GLOBAL_AUTO_INJECT
			= false;
#else
			= true;
#endif

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			globalScope = new GlobalDependencyScope();
		}
		#endregion

		/// <inheritdoc />
		public IDependencyContainer DependencyContainer { get; private set; } = new DependencyContainer();

		private GlobalDependencyScope()
		{
			InitializeGlobalContainer();
			this.InitializeGlobalScope();
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		/// <summary>
		/// Injects the currently active scene with the global resources.
		/// </summary>
		public void Inject()
		{
			SceneManager.GetActiveScene().GetRootGameObjects().Inject(DependencyContainer, true);
		}

		private void InitializeGlobalContainer()
		{
			IEnumerable<MethodInfo> containerProviderMethods = FindStaticMethods(typeof(GlobalContainerProviderAttribute));

			if (containerProviderMethods.Any())
			{
				containerProviderMethods = containerProviderMethods.OrderByDescending(m => m.GetCustomAttribute<GlobalContainerProviderAttribute>(false).Priority);
				MethodInfo containerProviderMethod = containerProviderMethods.First();

				if (containerProviderMethod.ReturnType == null)
				{
					throw new DependencyInjectionException("The method '{0}' defined on type '{1}' is marked as a dependency container provider for the global dependency scope, but does has no return value defined.", containerProviderMethod.Name, containerProviderMethod.DeclaringType.FullName);
				}
				else if (!typeof(IDependencyContainer).IsAssignableFrom(containerProviderMethod.ReturnType))
				{
					throw new DependencyInjectionException("The method '{0}' defined on type '{1}' is marked as a dependency container provider for the global dependency scope, but the return type is not assignable from type '{2}'.", containerProviderMethod.Name, containerProviderMethod.DeclaringType.FullName, typeof(IDependencyContainer).Name);
				}

				DependencyContainer = containerProviderMethod.Invoke(null, null) as IDependencyContainer;
			}
			else
			{
				DependencyContainer = new DependencyContainer();
			}
		}

		private void InitializeGlobalScope()
		{
			// Find all global scope initialization methods.
			IEnumerable<MethodInfo> scopeInitMethods = FindStaticMethods(typeof(GlobalScopeInstallerAttribute));
			scopeInitMethods = scopeInitMethods.OrderBy(m => m.GetCustomAttribute<GlobalScopeInstallerAttribute>(false).InstallPriority);

			// Parameter list used to invoke the global scope installers.
			object[] paramList = new object[] { DependencyContainer };

			Type currentContainerType = DependencyContainer.GetType();
			foreach (MethodInfo scopeInitMethod in scopeInitMethods)
			{
				ParameterInfo[] parameters = scopeInitMethod.GetParameters();

				// Perform final check on scope installer method parameters.
				if ((parameters.Length == 1) && parameters[0].ParameterType.IsAssignableFrom(currentContainerType))
				{
					scopeInitMethod.Invoke(null, paramList);
				}
			}
		}

		private IEnumerable<MethodInfo> FindStaticMethods(Type attributeType)
		{
			List<MethodInfo> methods = new List<MethodInfo>();
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type type in assembly.GetTypes())
				{
					foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
					{
						if (Attribute.IsDefined(method, attributeType, false))
						{
							methods.Add(method);
						}
					}
				}
			}

			return methods;
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode sceneLoadMode)
		{
			if (AutoInjectLoadedScenes)
			{
				scene.GetRootGameObjects().Inject(DependencyContainer, true);
			}
		}
	}
}