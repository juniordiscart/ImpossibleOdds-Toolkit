namespace ImpossibleOdds.DependencyInjection
{
	using System;
	using System.Collections.Generic;

	public interface IDependencyContainer
	{
		IDictionary<Type, IDependencyBinding> Bindings
		{
			get;
		}

		bool BindingExists<T>();
		bool BindingExists(Type bindingType);

		IDependencyBinding GetBinding<T>();
		IDependencyBinding GetBinding(Type bindingType);

		void RemoveBinding<T>();
		void RemoveBinding(Type bindingType);

		void Bind(IDependencyBinding binding);
		void Bind<T>(IDependencyBinding binding);
		void Bind(Type bindingType, IDependencyBinding binding);

		void BindWithInterfaces(IDependencyBinding binding);
		void BindWithInterfaces(Type bindingType, IDependencyBinding binding);
		void BindWithInterfaces<T>(IDependencyBinding binding);
	}
}
