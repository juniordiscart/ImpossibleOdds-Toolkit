using System;

namespace ImpossibleOdds
{
	/// <summary>
	/// Explicitly sets the script execution order to the specified value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ExecuteAtAttribute : Attribute
	{
		public int Order { get; }

		public ExecuteAtAttribute(int order)
		{
			Order = order;
		}
	}
}