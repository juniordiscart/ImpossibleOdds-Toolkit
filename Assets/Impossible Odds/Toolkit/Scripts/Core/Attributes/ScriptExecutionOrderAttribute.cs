namespace ImpossibleOdds
{
	using System;

	/// <summary>
	/// Defines the desired script execution order.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ScriptExecutionOrderAttribute : Attribute
	{
		public readonly int order;

		public ScriptExecutionOrderAttribute(int order)
		{
			this.order = order;
		}
	}
}