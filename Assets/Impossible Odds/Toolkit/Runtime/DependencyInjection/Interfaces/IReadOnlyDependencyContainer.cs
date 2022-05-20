namespace ImpossibleOdds.DependencyInjection
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Read-only access to check on and get bindings in a dependency resources container.
	/// </summary>
	public interface IReadOnlyDependencyContainer : IEnumerable<KeyValuePair<Type, IDependencyBinding>>
	{
		/// <summary>
		/// Checks whether a binding exists for the given type.
		/// </summary>
		/// <typeparam name="TypeKey">The type to check the binding for.</typeparam>
		/// <returns>True if the binding exists.</returns>
		bool BindingExists<TypeKey>();
		/// <summary>
		/// Checks whether a binding exists for the given type.
		/// </summary>
		/// <param name="typeKey">The type to check the binding for.</param>
		/// <returns>True if the binding exists.</returns>
		bool BindingExists(Type typeKey);

		/// <summary>
		/// Retrieves the binding for type the given type.
		/// </summary>
		/// <typeparam name="TypeKey">The type for which to get the binding.</typeparam>
		/// <returns>The binding, if it exists.</returns>
		IDependencyBinding GetBinding<TypeKey>();
		/// <summary>
		/// Retrieves the binding for the given type.
		/// </summary>
		/// <param name="typeKey">The type for which to get the binding.</param>
		/// <returns>The binding, if it exists.</returns>
		IDependencyBinding GetBinding(Type typeKey);
	}
}
