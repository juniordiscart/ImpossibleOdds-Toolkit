﻿namespace ImpossibleOdds.DependencyInjection
{
	using System;

	/// <summary>
	/// Defines that a class field or method should be injected.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public sealed class InjectAttribute : Attribute
	{ }
}
