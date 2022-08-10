namespace ImpossibleOdds
{
	using System;

	/// <summary>
	/// Explicitly sets the script execution order to the specified value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ExecuteAtAttribute : Attribute
	{
		private readonly int order;

		public int Order
		{
			get => order;
		}

		public ExecuteAtAttribute(int order)
		{
			this.order = order;
		}
	}
}
