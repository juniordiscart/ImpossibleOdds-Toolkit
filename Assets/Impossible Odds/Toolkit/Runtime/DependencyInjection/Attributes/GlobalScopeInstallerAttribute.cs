using System;

namespace ImpossibleOdds.DependencyInjection
{
	/// <summary>
	/// Defines a static method as a global dependency scope installer.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class GlobalScopeInstallerAttribute : Attribute
	{
		public int InstallPriority { get; }

		public GlobalScopeInstallerAttribute()
		{ }

		public GlobalScopeInstallerAttribute(int installPriority)
		{
			InstallPriority = installPriority;
		}
	}
}