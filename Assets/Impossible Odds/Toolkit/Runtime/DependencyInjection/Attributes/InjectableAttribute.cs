using System;

namespace ImpossibleOdds.DependencyInjection
{
	/// <summary>
	/// Defines that a class is injectable.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class InjectableAttribute : Attribute
	{ }
}