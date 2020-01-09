namespace ImpossibleOdds.DataMapping
{
	using System;

	/// <summary>
	/// Base interface for type resolvement interfaces.
	/// </summary>
	public interface IMappingTypeResolveParameter
	{
		/// <summary>
		/// The target type defined for the type resolvement.
		/// </summary>
		Type Target
		{
			get;
		}
	}
}
