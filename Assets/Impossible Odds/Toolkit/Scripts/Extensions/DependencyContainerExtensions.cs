using System;
using ImpossibleOdds;
using ImpossibleOdds.DependencyInjection;

/// <summary>
/// Extension class for dependency container classes.
/// This class is made partial for easy extending in parent projects.
/// </summary>
public static partial class DependencyContainerExtensions
{
	public static void RegisterInstance<T>(this IDependencyContainer container, T instance)
	{
		container.ThrowIfNull(nameof(container));
		container.Register<T>(new InstanceBinding<T>(instance));
	}

	public static void RegisterInstanceWithInterfaces<T>(this IDependencyContainer container, T instance)
	{
		container.ThrowIfNull(nameof(container));
		container.RegisterWithInterfaces<T>(new InstanceBinding<T>(instance));
	}

	public static void RegisterGenerator<T>(this IDependencyContainer container, Func<T> generator)
	{
		container.ThrowIfNull(nameof(container));
		container.Register<T>(new GeneratorBinding<T>(generator));
	}

	public static void RegisterGeneratorWithInterfaces<T>(this IDependencyContainer container, Func<T> generator)
	{
		container.ThrowIfNull(nameof(container));
		container.RegisterWithInterfaces<T>(new GeneratorBinding<T>(generator));
	}
}
