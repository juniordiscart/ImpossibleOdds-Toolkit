namespace ImpossibleOdds.DependencyInjection
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using UnityEngine;
	using UnityEngine.SceneManagement;

	public class GlobalDependencyScope : IDependencyScope
	{
		private static GlobalDependencyScope globalScope = null;

		internal static IDependencyScope GlobalScope
		{
			get { return globalScope; }
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitializeGlobalScope()
		{
			globalScope = new GlobalDependencyScope();
		}

		private IDependencyContainer globalContainer = new DependencyContainer();

		public IDependencyContainer DependencyContainer
		{
			get { return globalContainer; }
		}

		private GlobalDependencyScope()
		{
			InitializeGlobalContainer();
			InitializeGlobalScopes();
			SceneManager.sceneLoaded += OnSceneLoaded;
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

				globalContainer = containerProviderMethod.Invoke(null, null) as IDependencyContainer;
			}
			else
			{
				globalContainer = new DependencyContainer();
			}
		}

		private void InitializeGlobalScopes()
		{
			// Find all global scope initialization methods
			IEnumerable<MethodInfo> scopeInitMethods = FindStaticMethods(typeof(GlobalScopeInstallerAttribute));
			scopeInitMethods = scopeInitMethods.OrderBy(m => m.GetCustomAttribute<GlobalScopeInstallerAttribute>(false).InstallPriority);

			// Parameter list used to invoke the global scope installers
			object[] paramList = new object[] { globalContainer };

			Type currentContainerType = globalContainer.GetType();
			foreach (MethodInfo scopeInitMethod in scopeInitMethods)
			{
				ParameterInfo[] parameters = scopeInitMethod.GetParameters();

				// Perform final check on scope installer method parameters
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
						if (method.IsDefined(attributeType, false))
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
			Log.Info("Injecting scene '{0}' with the global dependency scope.", scene.name);
			scene.GetRootGameObjects().Inject(DependencyContainer, true);
		}
	}
}
