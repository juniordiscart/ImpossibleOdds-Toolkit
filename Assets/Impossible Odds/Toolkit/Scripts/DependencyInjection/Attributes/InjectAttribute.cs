namespace ImpossibleOdds.DependencyInjection
{
	using System;

	/// <summary>
	/// Defines that a field, property or method should be injected.
	/// Can optionally be set with an ID to restrict injections for certain sources.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
	public sealed class InjectAttribute : Attribute
	{
		private readonly string injectID = null;

		public string InjectID
		{
			get { return injectID; }
		}

		public bool HasInjectionIDSet
		{
			get { return !string.IsNullOrEmpty(injectID); }
		}

		public InjectAttribute()
		{ }

		public InjectAttribute(string injectID)
		{
			this.injectID = injectID;
		}
	}
}
