namespace ImpossibleOdds.DependencyInjection
{
	using System;

	/// <summary>
	/// Defines a static method as a global dependency container provider.
	/// Only The scope container provider with the highest priority value will
	/// will be invoked to provide the dependency container.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class GlobalContainerProviderAttribute : Attribute
	{
		private readonly int priority = 0;

		public int Priority
		{
			get => priority;
		}

		public GlobalContainerProviderAttribute()
		{ }

		public GlobalContainerProviderAttribute(int priority)
		{
			this.priority = priority;
		}
	}
}
