namespace ImpossibleOdds.Weblink.Caching
{
	using System;
	using ImpossibleOdds.ReflectionCaching;

	public interface IWeblinkReflectionMap : IReflectionMap
	{
		/// <summary>
		/// Get the targeted callbacks with the given attribute type.
		/// </summary>
		/// <param name="attributeType">The type of the attribute defining a targeted callback.</param>
		/// <returns>The set of targeted callbacks on the type.</returns>
		ITargetedCallback[] GetTargetedCallbacks(Type attributeType);
	}
}
