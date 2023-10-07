using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace ImpossibleOdds.DependencyInjection
{
	public class DependencyContainer : IDependencyContainer
	{
		private readonly ConcurrentDictionary<Type, IDependencyBinding> bindings = new ConcurrentDictionary<Type, IDependencyBinding>();

		/// <inheritdoc />
		public IDictionary<Type, IDependencyBinding> Bindings => bindings;

		/// <inheritdoc />
		public IEnumerator<KeyValuePair<Type, IDependencyBinding>> GetEnumerator()
		{
			return bindings.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <inheritdoc />
		public void Register(IDependencyBinding binding)
		{
			binding.ThrowIfNull(nameof(binding));
			Register(binding.GetTypeBinding(), binding);
		}

		/// <inheritdoc />
		public void Register<TTypeKey>(IDependencyBinding binding)
		{
			Register(typeof(TTypeKey), binding);
		}

		/// <inheritdoc />
		public void Register(Type typeKey, IDependencyBinding binding)
		{
			typeKey.ThrowIfNull(nameof(typeKey));
			binding.ThrowIfNull(nameof(binding));

			if (!typeKey.IsAssignableFrom(binding.GetTypeBinding()))
			{
				throw new DependencyInjectionException("Type key {0} is not assignable from type {1} as reported by {2}.", typeKey.Name, binding.GetTypeBinding().Name, nameof(binding));
			}

			bindings.AddOrUpdate(typeKey, binding, (t, b) => binding);
		}

		/// <inheritdoc />
		public void RegisterWithInterfaces(IDependencyBinding binding)
		{
			binding.ThrowIfNull(nameof(binding));
			RegisterWithInterfaces(binding.GetTypeBinding(), binding);
		}

		/// <inheritdoc />
		public void RegisterWithInterfaces<T>(IDependencyBinding binding)
		{
			RegisterWithInterfaces(typeof(T), binding);
		}

		/// <inheritdoc />
		public void RegisterWithInterfaces(Type typeKey, IDependencyBinding binding)
		{
			typeKey.ThrowIfNull(nameof(typeKey));
			binding.ThrowIfNull(nameof(binding));

			IEnumerable<Type> bindingTypes = DependencyInjectionUtilities.GetTypeAndInterfaces(typeKey);
			foreach (Type t in bindingTypes)
			{
				Register(t, binding);
			}
		}

		/// <inheritdoc />
		public bool BindingExists<TTypeKey>()
		{
			return BindingExists(typeof(TTypeKey));
		}

		/// <inheritdoc />
		public bool BindingExists(Type typeKey)
		{
			typeKey.ThrowIfNull(nameof(typeKey));
			return bindings.ContainsKey(typeKey);
		}

		/// <inheritdoc />
		public IDependencyBinding GetBinding<TTypeKey>()
		{
			return GetBinding(typeof(TTypeKey));
		}

		/// <inheritdoc />
		public IDependencyBinding GetBinding(Type typeKey)
		{
			typeKey.ThrowIfNull(nameof(typeKey));
			return bindings.TryGetValue(typeKey, out IDependencyBinding binding) ? binding : null;
		}

		/// <inheritdoc />
		public bool Remove<TTypeKey>()
		{
			return Remove(typeof(TTypeKey));
		}

		/// <inheritdoc />
		public bool Remove(Type typeKey)
		{
			typeKey.ThrowIfNull(nameof(typeKey));
			return bindings.TryRemove(typeKey, out _);
		}

		/// <summary>
		/// Get an instance of the requested type from a registered binding.
		/// </summary>
		/// <param name="typeOfObject">Type of object to look for in the resources container.</param>
		/// <returns>An instance of the requested type if a binding exists, null otherwise.</returns>
		public object GetInstance(Type typeOfObject)
		{
			return BindingExists(typeOfObject) ? GetBinding(typeOfObject).GetInstance() : null;
		}

		/// <summary>
		/// Get an instance of the requested type from a registered binding.
		/// </summary>
		/// <typeparam name="TTypeKey">Type of object to look for in the resources container.</typeparam>
		/// <returns>An instance of the requested type if a binding exists, null otherwise.</returns>
		public TTypeKey GetInstance<TTypeKey>()
		{
			return (TTypeKey)GetInstance(typeof(TTypeKey));
		}
	}
}