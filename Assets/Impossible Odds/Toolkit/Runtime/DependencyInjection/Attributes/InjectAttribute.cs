using System;
using System.Collections.Generic;

namespace ImpossibleOdds.DependencyInjection
{
	/// <summary>
	/// Defines that a field, property or method should be injected.
	/// Can optionally be set with an ID to restrict injections for certain sources.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = true)]
	public sealed class InjectAttribute : Attribute
	{
		private readonly HashSet<string> injectionNames = null;

		public bool HasNamedInjections => injectionNames != null;

		public IReadOnlyCollection<string> InjectionIds => injectionNames;

		public InjectAttribute()
		{ }

		public InjectAttribute(params string[] injectionIds)
		{
			this.injectionNames = new HashSet<string>(injectionIds);
		}

		public bool IsInjectionIdDefined(string injectionId)
		{
			if (string.IsNullOrEmpty(injectionId))
			{
				return (injectionNames == null) || injectionNames.Contains(string.Empty);
			}

			return injectionNames?.Contains(injectionId) ?? false;
		}
	}
}