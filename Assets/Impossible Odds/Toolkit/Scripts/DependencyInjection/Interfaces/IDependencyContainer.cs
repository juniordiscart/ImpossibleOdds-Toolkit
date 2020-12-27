namespace ImpossibleOdds.DependencyInjection
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Dependency container interface.
	/// </summary>
	public interface IDependencyContainer : IEnumerable<KeyValuePair<Type, IDependencyBinding>>
	{
		/// <summary>
		/// Does a binding exist for type T?
		/// </summary>
		/// <typeparam name="T">The type to check the binding for.</typeparam>
		/// <returns>True if the binding exists.</returns>
		bool BindingExists<T>();
		/// <summary>
		/// Does a binding exist for type bindingType?
		/// </summary>
		/// <param name="bindingType">The type to check the binding for.</param>
		/// <returns>True if the binding exists.</returns>
		bool BindingExists(Type bindingType);

		/// <summary>
		/// Retrieves the binding for type T.
		/// </summary>
		/// <typeparam name="T">The type for which to get the binding.</typeparam>
		/// <returns>The binding, if it exists.</returns>
		IDependencyBinding GetBinding<T>();
		/// <summary>
		/// Retrieves the binding for type bindingType.
		/// </summary>
		/// <param name="bindingType">The type for which to get the binding.</param>
		/// <returns>The binding, if it exists.</returns>
		IDependencyBinding GetBinding(Type bindingType);

		/// <summary>
		/// Remove the binding for type T.
		/// </summary>
		/// <typeparam name="T">The type for which to remove the binding.</typeparam>
		void RemoveBinding<T>();
		/// <summary>
		/// Remove the binding for type bindingType.
		/// </summary>
		/// <param name="bindingType">The type for which to remove the binding.</param>
		void RemoveBinding(Type bindingType);

		/// <summary>
		/// Bind the type reported by the binding to the binding itself.
		/// </summary>
		/// <param name="binding">The binding to bind.</param>
		void Bind(IDependencyBinding binding);
		/// <summary>
		/// Bind the binding to type T. The binding's reported type should be assignable to T.
		/// </summary>
		/// <param name="binding">The binding to bind.</param>
		/// <typeparam name="T">The type to which the binding is to be bound.</typeparam>
		void Bind<T>(IDependencyBinding binding);
		/// <summary>
		/// Bind the binding to type bindingType. The binding's reported type should be assignable to bindingType.
		/// </summary>
		/// <param name="bindingType">The type to which the binding is to be bound.</param>
		/// <param name="binding">The binding to bind.</param>
		void Bind(Type bindingType, IDependencyBinding binding);

		/// <summary>
		/// Bind the type reported by the binding and all its implemented interfaces to itself.
		/// </summary>
		/// <param name="binding">The binding to bind.</param>
		void BindWithInterfaces(IDependencyBinding binding);
		/// <summary>
		///
		/// </summary>
		/// <param name="binding"></param>
		/// <typeparam name="T"></typeparam>
		void BindWithInterfaces<T>(IDependencyBinding binding);
		/// <summary>
		/// Bind the binding to type bindingType and its implemented interfaces. The binding's reported type should be assignable to the bindingType.
		/// </summary>
		/// <param name="bindingType">The type to which the binding is to be bound.</param>
		/// <param name="binding">The binding to bind.</param>
		void BindWithInterfaces(Type bindingType, IDependencyBinding binding);
	}
}
