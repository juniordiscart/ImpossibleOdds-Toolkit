using System;
using ImpossibleOdds;
using ImpossibleOdds.DependencyInjection;

/// <summary>
/// Extension class for dependency container classes.
/// This class is made partial for easy extending in parent projects.
/// </summary>
public static partial class DependencyContainerExtensions
{
	/// <summary>
	/// Registers an instance to the resource container.
	/// </summary>
	/// <param name="container">The container with resources.</param>
	/// <param name="instance">Instance resource to bind to the container.</param>
	/// <typeparam name="T">Type of the instance being bound.</typeparam>
	public static void RegisterInstance<T>(this IDependencyContainer container, T instance)
	{
		container.ThrowIfNull(nameof(container));
		container.Register<T>(new InstanceBinding<T>(instance));
	}

	/// <summary>
	/// Registers an instance along with all of its implemented interfaces to the resource container.
	/// </summary>
	/// <param name="container">The container with resources.</param>
	/// <param name="instance">Instance resource to bind to the container.</param>
	/// <typeparam name="T">Type of the instance being bound.</typeparam>
	public static void RegisterInstanceWithInterfaces<T>(this IDependencyContainer container, T instance)
	{
		container.ThrowIfNull(nameof(container));
		container.RegisterWithInterfaces<T>(new InstanceBinding<T>(instance));
	}

	/// <summary>
	/// Registers a generator to the resource container.
	/// </summary>
	/// <param name="container">The container with resources.</param>
	/// <param name="generator">The generator to bind to the container.</param>
	/// <typeparam name="T">Type of value the generator provides when requesting an instance.</typeparam>
	public static void RegisterGenerator<T>(this IDependencyContainer container, Func<T> generator)
	{
		container.ThrowIfNull(nameof(container));
		container.Register<T>(new GeneratorBinding<T>(generator));
	}

	/// <summary>
	/// Registers a generator to the resource container with all of the implemented interfaces by the type of resource then generator creates.
	/// </summary>
	/// <param name="container">The container with resources.</param>
	/// <param name="generator">The generator to bind to the container.</param>
	/// <typeparam name="T">Type of value the generator provides when requesting an instance.</typeparam>
	public static void RegisterGeneratorWithInterfaces<T>(this IDependencyContainer container, Func<T> generator)
	{
		container.ThrowIfNull(nameof(container));
		container.RegisterWithInterfaces<T>(new GeneratorBinding<T>(generator));
	}
}
