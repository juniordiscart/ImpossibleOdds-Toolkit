namespace ImpossibleOdds.DependencyInjection
{
	using System;

	/// <summary>
	/// Dependency container interface.
	/// </summary>
	public interface IDependencyContainer : IReadOnlyDependencyContainer
	{
		/// <summary>
		/// Remove the binding for the given type.
		/// </summary>
		/// <typeparam name="T">The type for which to remove the binding.</typeparam>
		/// <returns>True, when the binding got removed successfully.</returns>
		bool Remove<T>();
		/// <summary>
		/// Remove the binding for the given type.
		/// </summary>
		/// <param name="typeKey">The type for which to remove the binding.</param>
		/// <returns>True, when the binding got removed successfully.</returns>
		bool Remove(Type typeKey);

		/// <summary>
		/// Register the type reported by the binding to the binding itself.
		/// </summary>
		/// <param name="binding">The binding to bind.</param>
		void Register(IDependencyBinding binding);
		/// <summary>
		/// Register the binding to the given type. The binding's reported type should be assignable to the given type.
		/// </summary>
		/// <param name="binding">The binding to bind.</param>
		/// <typeparam name="T">The type to which the binding is to be bound.</typeparam>
		void Register<T>(IDependencyBinding binding);
		/// <summary>
		/// Register the binding to the given type. The binding's reported type should be assignable to the given type.
		/// </summary>
		/// <param name="typeKey">The type to which the binding is to be bound.</param>
		/// <param name="binding">The binding to bind.</param>
		void Register(Type typeKey, IDependencyBinding binding);

		/// <summary>
		/// Register the binding to its reported type and all of its implemented interfaces.
		/// </summary>
		/// <param name="binding">The binding to register.</param>
		void RegisterWithInterfaces(IDependencyBinding binding);
		/// <summary>
		/// Register the binding to the type and its implemented interfaces. The binding's reported type should be assignable to the given type.
		/// </summary>
		/// <param name="binding">The binding to register.</param>
		/// <typeparam name="TypeKey">The type key to which the binding is to be registered.</typeparam>
		void RegisterWithInterfaces<TypeKey>(IDependencyBinding binding);
		/// <summary>
		/// Register the binding to the type and its implemented interfaces. The binding's reported type should be assignable to the given type.
		/// </summary>
		/// <param name="typeKey">The type key to which the binding is to be registered.</param>
		/// <param name="binding">The binding to register.</param>
		void RegisterWithInterfaces(Type typeKey, IDependencyBinding binding);
	}
}
