namespace ImpossibleOdds.DependencyInjection
{
	using System;

	/// <summary>
	/// Defines a static method as a global dependency context installer.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class GlobalContextInstallerAttribute : Attribute
	{
		private readonly int installPriority = 0;

		public int InstallPriority
		{
			get { return installPriority; }
		}

		public GlobalContextInstallerAttribute()
		{ }

		public GlobalContextInstallerAttribute(int installPriority)
		{
			this.installPriority = installPriority;
		}
	}
}
