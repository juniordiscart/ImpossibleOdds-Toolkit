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
		public static void Inject(this GameObject gameObject, IReadOnlyDependencyContainer container, bool includeChildren = false)
		{
			gameObject.ThrowIfNull(nameof(gameObject));
			container.ThrowIfNull(nameof(container));
			IEnumerable<MonoBehaviour> components = includeChildren ? gameObject.GetComponentsInChildren<MonoBehaviour>(true) : gameObject.GetComponents<MonoBehaviour>();
			DependencyInjector.Inject(container, components);
		}

		/// <summary>
		/// Inject the components found on the GameObject. Optionally includes the children as well.
		/// </summary>
		/// <param name="gameObject">The GameObject of which the components will be injected.</param>
		/// <param name="container">Container containing resources that can be injected.</param>
		/// <param name="injectionId">Name of the scope these resources belong to.</param>
		/// <param name="includeChildren">Include components found in children of the GameObject.</param>
		public static void Inject(this GameObject gameObject, IReadOnlyDependencyContainer container, string injectionId, bool includeChildren = false)
		{
			gameObject.ThrowIfNull(nameof(gameObject));
			container.ThrowIfNull(nameof(container));
			injectionId.ThrowIfNullOrEmpty(nameof(injectionId));

			IEnumerable<MonoBehaviour> components = includeChildren ? gameObject.GetComponentsInChildren<MonoBehaviour>(true) : gameObject.GetComponents<MonoBehaviour>();
			DependencyInjector.Inject(container, injectionId, components);
		}

		/// <summary>
		/// Inject the components found in the collection of GameObjects. Optionally includes their children as well.
		/// </summary>
		/// <param name="gameObjects">The GameObjects of which the components will be injected.</param>
		/// <param name="container">Container containing resources that can be injected.</param>
		/// <param name="includeChildren">Include components found in children of the GameObjects.</param>
		public static void Inject(this IEnumerable<GameObject> gameObjects, IReadOnlyDependencyContainer container, bool includeChildren = false)
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
		/// <param name="injectionId">Name of the scope these resources belong to.</param>
		/// <param name="includeChildren">Include components found in children of the GameObjects.</param>
		public static void Inject(this IEnumerable<GameObject> gameObjects, IReadOnlyDependencyContainer container, string injectionId, bool includeChildren = false)
		{
			gameObjects.ThrowIfNull(nameof(gameObjects));
			foreach (GameObject gameObject in gameObjects)
			{
				gameObject.Inject(container, injectionId, includeChildren);
			}
		}

		/// <summary>
		/// Inject the component with the resources found in the dependency container.
		/// </summary>
		/// <param name="component">Component to be injected</param>
		/// <param name="container">Container containing resources that can be injected.</param>
		public static void Inject(this MonoBehaviour component, IReadOnlyDependencyContainer container)
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
		/// <param name="injectionId">Name of the scope these resources belong to.</param>
		public static void Inject(this MonoBehaviour component, IReadOnlyDependencyContainer container, string injectionId)
		{
			component.ThrowIfNull(nameof(component));
			container.ThrowIfNull(nameof(container));
			injectionId.ThrowIfNullOrEmpty(nameof(injectionId));
			DependencyInjector.Inject(container, injectionId, component);
		}

		/// <summary>
		/// Inject the entire colletion of components with the resources found in the dependency container.
		/// </summary>
		/// <param name="components">Components to be injected.</param>
		/// <param name="container">Container containing resources that can be injected.</param>
		public static void Inject(this IEnumerable<MonoBehaviour> components, IReadOnlyDependencyContainer container)
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
		/// <param name="injectionId">Name of the scope these resources belong to.</param>
		public static void Inject(this IEnumerable<MonoBehaviour> components, IReadOnlyDependencyContainer container, string injectionId)
		{
			components.ThrowIfNull(nameof(components));
			container.ThrowIfNull(nameof(container));
			injectionId.ThrowIfNullOrEmpty(nameof(injectionId));
			DependencyInjector.Inject(container, injectionId, components);
		}

		/// <summary>
		/// Adds a component of desired type to the game object, and injects it with the given dependency container.
		/// </summary>
		/// <param name="gameObject">The game object to add the component to.</param>
		/// <param name="container">Container containing resources that can be injected.</param>
		/// <typeparam name="TComponent">Type of component to tadd to the game object.</typeparam>
		/// <returns>The component, injected with resources.</returns>
		public static TComponent AddComponentAndInject<TComponent>(this GameObject gameObject, IReadOnlyDependencyContainer container)
		where TComponent : MonoBehaviour
		{
			return gameObject.AddComponentAndInject<TComponent>(container, string.Empty);
		}

		/// <summary>
		/// Adds a component of the desired type to the game object and injects it with
		/// the resources found in the container, under the given injection identifier.
		/// </summary>
		/// <param name="gameObject">The game object to add the component to.</param>
		/// <param name="container">Container containing resources that can be injected.</param>
		/// <param name="injectionId">Name of the scope these resources belong to.</param>
		/// <typeparam name="TComponent">Type of component to tadd to the game object.</typeparam>
		/// <returns>The component, injected with resources.</returns>
		public static TComponent AddComponentAndInject<TComponent>(this GameObject gameObject, IReadOnlyDependencyContainer container, string injectionId)
		where TComponent : MonoBehaviour
		{
			gameObject.ThrowIfNull(nameof(gameObject));
			container.ThrowIfNull(nameof(container));

			TComponent c = gameObject.AddComponent<TComponent>();
			c.Inject(container, injectionId);

			return c;
		}
	}
}
