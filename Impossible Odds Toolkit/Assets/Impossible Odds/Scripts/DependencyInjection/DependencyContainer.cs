namespace ImpossibleOdds.DependencyInjection
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	public class DependencyContainer : IDependencyContainer
	{
		private Dictionary<Type, IDependencyBinding> bindings = new Dictionary<Type, IDependencyBinding>();

		public IDictionary<Type, IDependencyBinding> Bindings
		{
			get { return bindings; }
		}

		public void Bind(IDependencyBinding binding)
		{
			if (binding == null)
			{
				throw new ArgumentNullException("binding");
			}

			Bind(binding.GetTypeBinding(), binding);
		}

		public void Bind<T>(IDependencyBinding binding)
		{
			Bind(typeof(T), binding);
		}

		public void Bind(Type bindingType, IDependencyBinding binding)
		{
			if (bindingType == null)
			{
				throw new ArgumentNullException("bindingType");
			}
			else if (binding == null)
			{
				throw new ArgumentNullException("binding");
			}

			if (bindings.ContainsKey(bindingType))
			{
#if IMPOSSIBLE_ODDS_VERBOSE
				Debug.LogWarningFormat("A binding for type {0} already exists. Override with new binding.", bindingType.Name);
#endif
				bindings[bindingType] = binding;
			}
			else
			{
				bindings.Add(bindingType, binding);
			}
		}

		public void BindWithInterfaces(IDependencyBinding binding)
		{
			if (binding == null)
			{
				throw new ArgumentNullException("binding");
			}

			BindWithInterfaces(binding.GetTypeBinding(), binding);
		}

		public void BindWithInterfaces<T>(IDependencyBinding binding)
		{
			BindWithInterfaces(typeof(T), binding);
		}

		public void BindWithInterfaces(Type bindingType, IDependencyBinding binding)
		{
			if (bindingType == null)
			{
				throw new ArgumentNullException("bindingType");
			}
			else if (binding == null)
			{
				throw new ArgumentNullException("binding");
			}

			IEnumerable<Type> bindingTypes = DependencyInjectionUtilities.GetTypeAndInterfaces(bindingType);
			foreach (Type t in bindingTypes)
			{
				Bind(t, binding);
			}
		}

		public bool BindingExists<T>()
		{
			return BindingExists(typeof(T));
		}

		public bool BindingExists(Type bindingType)
		{
			if (bindingType == null)
			{
				throw new ArgumentNullException("bindingType");
			}

			return bindings.ContainsKey(bindingType);
		}

		public IDependencyBinding GetBinding<T>()
		{
			return GetBinding(typeof(T));
		}

		public IDependencyBinding GetBinding(Type bindingType)
		{
			if (bindingType == null)
			{
				throw new ArgumentNullException("bindingType");
			}

			return bindings.ContainsKey(bindingType) ? bindings[bindingType] : null;
		}

		public void RemoveBinding<T>()
		{
			RemoveBinding(typeof(T));
		}

		public void RemoveBinding(Type bindingType)
		{
			if (bindingType == null)
			{
				throw new ArgumentNullException("bindingType");
			}

			bindings.Remove(bindingType);
		}
	}
}
