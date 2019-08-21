namespace ImpossibleOdds.DependencyInjection
{
	using System;
	using System.Collections.Generic;

	public interface IDependencyContainer
	{
		IDictionary<Type, IDependencyGenerator> Bindings
		{
			get;
		}

		bool BindingExists<T>();
		bool BindingExists(Type bindingType);

		IDependencyGenerator GetBinding<T>();
		IDependencyGenerator GetBinding(Type bindingType);

		void RemoveBinding<T>();
		void RemoveBinding(Type bindingType);

		void Bind(IDependencyGenerator binding);
		void Bind<T>(IDependencyGenerator binding);
		void Bind(Type bindingType, IDependencyGenerator binding);

		void BindWithInterfaces(IDependencyGenerator binding);
		void BindWithInterfaces(Type bindingType, IDependencyGenerator binding);
		void BindWithInterfaces<T>(IDependencyGenerator binding);
	}
}
