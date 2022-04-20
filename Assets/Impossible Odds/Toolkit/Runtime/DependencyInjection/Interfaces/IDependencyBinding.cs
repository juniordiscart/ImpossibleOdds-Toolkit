namespace ImpossibleOdds.DependencyInjection
{
	using System;

	/// <summary>
	/// Interface to bind a type to instances of that type.
	/// </summary>
	public interface IDependencyBinding
	{
		/// <summary>
		/// The type of the instances returned.
		/// </summary>
		/// <returns></returns>
		Type GetTypeBinding();

		/// <summary>
		/// Get an instance of the bound type.
		/// </summary>
		/// <returns>Instance of the bound type.</returns>
		object GetInstance();
	}
}
