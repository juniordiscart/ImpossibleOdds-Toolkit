using System;
using System.Collections.Generic;

namespace ImpossibleOdds.DependencyInjection
{
	/// <summary>
	/// Defines that a field, property or method should be injected.
	/// Can optionally be set with a name to restrict injections for certain sources.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = true)]
	public sealed class InjectAttribute : Attribute
	{
		private readonly HashSet<string> injectionNames = null;

		public bool HasNamedInjections => injectionNames != null;

		public IReadOnlyCollection<string> InjectionNames => injectionNames;

		public InjectAttribute()
		{ }

		public InjectAttribute(params string[] injectionNames)
		{
			this.injectionNames = new HashSet<string>(injectionNames);
		}

		public bool IsInjectionIdDefined(string injectionName)
		{
			if (string.IsNullOrEmpty(injectionName))
			{
				return (injectionNames == null) || injectionNames.Contains(string.Empty);
			}

			return injectionNames?.Contains(injectionName) ?? false;
		}
	}
}