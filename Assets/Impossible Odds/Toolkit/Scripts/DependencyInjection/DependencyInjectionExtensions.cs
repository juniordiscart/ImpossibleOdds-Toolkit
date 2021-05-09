namespace ImpossibleOdds.DependencyInjection
{
	using System.Collections.Generic;
	using UnityEngine;

	public static class DependencyInjectionExtensions
	{
		/// <summary>
		/// Inject the components found on the GameObject. Optionally includes the children as well.
		/// </summary>
		/// <param name="gameObject">The GameObject of which the components will be injected.</param>
		/// <param name="container">Container containing resources that can be injected.</param>
		/// <param name="includeChildren">Include components found in children of the GameObject.</param>
		public static void Inject(this GameObject gameObject, IDependencyContainer container, bool includeChildren = false)
		{
			gameObject.ThrowIfNull(nameof(gameObject));
			container.ThrowIfNull(nameof(container));
			IEnumerable<Component> components = includeChildren ? gameObject.GetComponentsInChildren<Component>(true) : gameObject.GetComponents<Component>();
			DependencyInjector.Inject(container, components);
		}

		/// <summary>
		/// Inject the components found on the GameObject. Optionally includes the children as well.
		/// </summary>
		/// <param name="gameObject">The GameObject of which the components will be injected.</param>
		/// <param name="container">Container containing resources that can be injected.</param>
		/// <param name="injectionID">Name of the scope these resources belong to.</param>
		/// <param name="includeChildren">Include components found in children of the GameObject.</param>
		public static void Inject(this GameObject gameObject, IDependencyContainer container, string injectionID, bool includeChildren = false)
		{
			gameObject.ThrowIfNull(nameof(gameObject));
			container.ThrowIfNull(nameof(container));
			injectionID.ThrowIfNullOrEmpty(nameof(injectionID));

			IEnumerable<Component> components = includeChildren ? gameObject.GetComponentsInChildren<Component>(true) : gameObject.GetComponents<Component>();
			DependencyInjector.Inject(container, injectionID, components);
		}

		/// <summary>
		/// Inject the components found in the collection of GameObjects. Optionally includes their children as well.
		/// </summary>
		/// <param name="gameObjects">The GameObjects of which the components will be injected.</param>
		/// <param name="container">Container containing resources that can be injected.</param>
		/// <param name="includeChildren">Include components found in children of the GameObjects.</param>
		public static void Inject(this IEnumerable<GameObject> gameObjects, IDependencyContainer container, bool includeChildren = false)
		{
			gameObjects.ThrowIfNull(nameof(gameObjects));
			foreach (GameObject gameObject in gameObjects)
			{
				gameObject.Inject(container, includeChildren);
			}
		}

		/// <summary>
		/// Inject the components found in the collection of GameObjects. Optionally includes their children as well.
		/// </summary>
		/// <param name="gameObjects">The GameObjects of which the components will be injected.</param>
		/// <param name="container">Container containing resources that can be injected.</param>
		/// <param name="injectionID">Name of the scope these resources belong to.</param>
		/// <param name="includeChildren">Include components found in children of the GameObjects.</param>
		public static void Inject(this IEnumerable<GameObject> gameObjects, IDependencyContainer container, string injectionID, bool includeChildren = false)
		{
			gameObjects.ThrowIfNull(nameof(gameObjects));
			foreach (GameObject gameObject in gameObjects)
			{
				gameObject.Inject(container, injectionID, includeChildren);
			}
		}

		/// <summary>
		/// Inject the component with the resources found in the dependency container.
		/// </summary>
		/// <param name="component">Component to be injected</param>
		/// <param name="container">Container containing resources that can be injected.</param>
		public static void Inject(this Component component, IDependencyContainer container)
		{
			component.ThrowIfNull(nameof(component));
			container.ThrowIfNull(nameof(container));
			DependencyInjector.Inject(container, component);
		}

		/// <summary>
		/// Inject the component with named resources found in the dependency container.
		/// Only resources marked with a matching name will get injected.
		/// </summary>
		/// <param name="component">Component to be injected.</param>
		/// <param name="container">Container containing resources that can be injected.</param>
		/// <param name="injectionID">Name of the scope these resources belong to.</param>
		public static void Inject(this Component component, IDependencyContainer container, string injectionID)
		{
			component.ThrowIfNull(nameof(component));
			container.ThrowIfNull(nameof(container));
			injectionID.ThrowIfNullOrEmpty(nameof(injectionID));
			DependencyInjector.Inject(container, injectionID, component);
		}

		/// <summary>
		/// Inject the entire colletion of components with the resources found in the dependency container.
		/// </summary>
		/// <param name="components">Components to be injected.</param>
		/// <param name="container">Container containing resources that can be injected.</param>
		public static void Inject(this IEnumerable<Component> components, IDependencyContainer container)
		{
			components.ThrowIfNull(nameof(components));
			container.ThrowIfNull(nameof(container));
			DependencyInjector.Inject(container, components);
		}

		/// <summary>
		/// Inject the entire colletion of components with named resources found in the dependency container.
		/// </summary>
		/// <param name="components">Components to be injected.</param>
		/// <param name="container">Container containing resources that can be injected.</param>
		/// <param name="injectionID">Name of the scope these resources belong to.</param>
		public static void Inject(this IEnumerable<Component> components, IDependencyContainer container, string injectionID)
		{
			components.ThrowIfNull(nameof(components));
			container.ThrowIfNull(nameof(container));
			injectionID.ThrowIfNullOrEmpty(nameof(injectionID));
			DependencyInjector.Inject(container, injectionID, components);
		}
	}
}
