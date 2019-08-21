namespace ImpossibleOdds.DataMapping
{
	using System;

	/// <summary>
	/// Defines that the field is required to be present in a data stream.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class RequiredAttribute : Attribute { }
}