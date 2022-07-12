namespace ImpossibleOdds.DependencyInjection
{
	using System;

	/// <summary>
	/// Defines a static method as a global dependency scope installer.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class GlobalScopeInstallerAttribute : Attribute
	{
		private readonly int installPriority = 0;

		public int InstallPriority
		{
			get => installPriority;
		}

		public GlobalScopeInstallerAttribute()
		{ }

		public GlobalScopeInstallerAttribute(int installPriority)
		{
			this.installPriority = installPriority;
		}
	}
}
