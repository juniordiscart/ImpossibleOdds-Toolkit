namespace ImpossibleOdds.DependencyInjection.Extensions
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	public static class DependencyInjectionExtensions
	{
		/// <summary>
		/// Inject all of the GameObject's directly attached components. Optionally injects all its children as well.
		/// </summary>
		/// <param name="gameObj">Game object to inject.</param>
		/// <param name="context">Context to use during injection.</param>
		/// <param name="includeChildren">When true, will recursively inject all of its children as well.</param>
		public static void Inject(this GameObject gameObj, IDependencyContext context, bool includeChildren = false)
		{
			context.ThrowIfNull(nameof(context));
			IEnumerable<Component> components = gameObj.GetComponents<Component>();
			components.Inject(context);

			if (includeChildren)
			{
				foreach (Transform f in gameObj.transform)
				{
					f.gameObject.Inject(context, includeChildren);
				}
			}
		}

		/// <summary>
		/// Inject a series of GameObjects in turn. Optionally injects each of their children as well.
		/// </summary>
		/// <param name="gameObjs">Game objects to inject.</param>
		/// <param name="context">Context to use during injection.</param>
		/// <param name="includeChildren">When true, will recursively inject all of their children as well.</param>
		public static void Inject(this IEnumerable<GameObject> gameObjs, IDependencyContext context, bool includeChildren = false)
		{
			foreach (GameObject gameObj in gameObjs)
			{
				gameObj.Inject(context, includeChildren);
			}
		}

		/// <summary>
		/// Inject the component.
		/// </summary>
		/// <param name="component">Component to inject.</param>
		/// <param name="context">Context to use during injection.</param>
		public static void Inject(this Component component, IDependencyContext context)
		{
			context.ThrowIfNull(nameof(context));
			DependencyInjector.Inject(context.DependencyContainer, component);
		}

		/// <summary>
		/// Inject a series of components.
		/// </summary>
		/// <param name="components">Components to inject.</param>
		/// <param name="context">Context to use during injection.</param>
		public static void Inject(this IEnumerable<Component> components, IDependencyContext context)
		{
			foreach (Component c in components)
			{
				c.Inject(context);
			}
		}
	}
}
