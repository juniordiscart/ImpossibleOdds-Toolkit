using System;

namespace ImpossibleOdds.DependencyInjection
{
	/// <summary>
	/// Defines a static method as a global dependency container provider.
	/// Only The scope container provider with the highest priority value will
	/// will be invoked to provide the dependency container.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class GlobalContainerProviderAttribute : Attribute
	{
		public int Priority { get; }

		public GlobalContainerProviderAttribute()
		{ }

		public GlobalContainerProviderAttribute(int priority)
		{
			Priority = priority;
		}
	}
}