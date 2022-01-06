namespace ImpossibleOdds.DependencyInjection
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public class DependencyContainer : IDependencyContainer
	{
		private Dictionary<Type, IDependencyBinding> bindings = new Dictionary<Type, IDependencyBinding>();

		/// <inheritdoc />
		public IDictionary<Type, IDependencyBinding> Bindings
		{
			get { return bindings; }
		}

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
		public void Register<TypeKey>(IDependencyBinding binding)
		{
			Register(typeof(TypeKey), binding);
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

			if (bindings.ContainsKey(typeKey))
			{
				Log.Warning("A binding for type {0} already exists. Overriding with new binding.", typeKey.Name);
			}

			bindings[typeKey] = binding;
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
		public bool BindingExists<TypeKey>()
		{
			return BindingExists(typeof(TypeKey));
		}

		/// <inheritdoc />
		public bool BindingExists(Type typeKey)
		{
			typeKey.ThrowIfNull(nameof(typeKey));
			return bindings.ContainsKey(typeKey);
		}

		/// <inheritdoc />
		public IDependencyBinding GetBinding<TypeKey>()
		{
			return GetBinding(typeof(TypeKey));
		}

		/// <inheritdoc />
		public IDependencyBinding GetBinding(Type typeKey)
		{
			typeKey.ThrowIfNull(nameof(typeKey));
			return bindings.ContainsKey(typeKey) ? bindings[typeKey] : null;
		}

		/// <inheritdoc />
		public bool Remove<TypeKey>()
		{
			return Remove(typeof(TypeKey));
		}

		/// <inheritdoc />
		public bool Remove(Type typeKey)
		{
			typeKey.ThrowIfNull(nameof(typeKey));
			return bindings.Remove(typeKey);
		}
	}
}
