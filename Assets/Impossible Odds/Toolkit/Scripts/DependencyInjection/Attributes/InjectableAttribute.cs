namespace ImpossibleOdds.DependencyInjection
{
	using System;

	/// <summary>
	/// Defines that a class is injectable.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class InjectableAttribute : Attribute
	{ }
}
