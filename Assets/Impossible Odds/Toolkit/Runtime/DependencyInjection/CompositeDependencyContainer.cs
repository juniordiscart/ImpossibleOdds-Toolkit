namespace ImpossibleOdds.DependencyInjection
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// A composite resource container that combines the resources of multiple others in one.
	/// </summary>
	public class CompositeDependencyContainer : IReadOnlyDependencyContainer, IEnumerable<IReadOnlyDependencyContainer>
	{
		private List<IReadOnlyDependencyContainer> containers = null;

		public CompositeDependencyContainer()
		{
			containers = new List<IReadOnlyDependencyContainer>();
		}

		public CompositeDependencyContainer(IEnumerable<IReadOnlyDependencyContainer> containers)
		{
			this.containers = new List<IReadOnlyDependencyContainer>(containers);
		}

		public CompositeDependencyContainer(params IReadOnlyDependencyContainer[] containers)
		{
			this.containers = new List<IReadOnlyDependencyContainer>(containers);
		}

		/// <summary>
		/// The ordered list of dependency containers found in this composite container.
		/// </summary>
		public IReadOnlyList<IReadOnlyDependencyContainer> ChildContainers
		{
			get => containers;
		}

		/// <summary>
		/// Adds a resource container to this composite container.
		/// </summary>
		/// <param name="container">The container to add.</param>
		public void AddContainer(IReadOnlyDependencyContainer container)
		{
			container.ThrowIfNull(nameof(container));

			if ((container != this) && !containers.Contains(container))
			{
				containers.Add(container);
			}
		}

		/// <summary>
		/// Remove a resource container from this composite container.
		/// Note: this will return false if a self-reference has been given.
		/// </summary>
		/// <param name="container">The container to remove.</param>
		/// <returns>True if the container was removed successfully, false otherwise.</returns>
		public bool RemoveContainer(IReadOnlyDependencyContainer container)
		{
			container.ThrowIfNull(nameof(container));
			return containers.Remove(container);
		}

		/// <summary>
		/// Check whether a specific container exists in this composite container.
		/// Note: this will return true if a self-reference has been given.
		/// </summary>
		/// <param name="container">The container to check for.</param>
		/// <returns>True if the container exists in the composite structure, false otherwise.</returns>
		public bool ContainerExists(IReadOnlyDependencyContainer container)
		{
			container.ThrowIfNull(nameof(container));
			return (container == this) || containers.Contains(container);
		}

		/// <inheritdoc />
		public bool BindingExists<TypeKey>()
		{
			return BindingExists(typeof(TypeKey));
		}

		/// <inheritdoc />
		public bool BindingExists(Type typeKey)
		{
			typeKey.ThrowIfNull(nameof(typeKey));
			foreach (IReadOnlyDependencyContainer c in containers)
			{
				if (c.BindingExists(typeKey))
				{
					return true;
				}
			}

			return false;
		}

		/// <inheritdoc />
		public IDependencyBinding GetBinding<TypeKey>()
		{
			return GetBinding(typeof(TypeKey));
		}

		/// <inheritdoc />
		public IDependencyBinding GetBinding(Type typeKey)
		{
			foreach (IReadOnlyDependencyContainer container in containers)
			{
				if (container.BindingExists(typeKey))
				{
					return container.GetBinding(typeKey);
				}
			}

			return null;
		}

		public IEnumerator<IReadOnlyDependencyContainer> GetEnumerator()
		{
			return containers.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator<KeyValuePair<Type, IDependencyBinding>> IEnumerable<KeyValuePair<Type, IDependencyBinding>>.GetEnumerator()
		{
			foreach (IReadOnlyDependencyContainer container in containers)
			{
				foreach (KeyValuePair<Type, IDependencyBinding> bindingPair in container)
				{
					yield return bindingPair;
				}
			}
		}
	}
}
