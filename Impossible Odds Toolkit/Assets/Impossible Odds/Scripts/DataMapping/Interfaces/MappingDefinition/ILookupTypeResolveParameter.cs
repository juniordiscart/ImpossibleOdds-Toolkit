namespace ImpossibleOdds.DataMapping
{
	using System;

	/// <summary>
	/// Defines a specific type resolve in a lookup data structure.
	/// </summary>
	public interface ILookupTypeResolveParameter
	{
		/// <summary>
		/// The target type defined for the type resolvement.
		/// </summary>
		/// <value></value>
		Type Target
		{
			get;
		}

		/// <summary>
		/// The key that the type resolve information will be stored as in the lookup data structure.
		/// </summary>
		/// <value></value>
		object Key
		{
			get;
		}

		/// <summary>
		/// The value that uniquely links to the target type in the lookup data structure.
		/// </summary>
		/// <value></value>
		object Value
		{
			get;
		}
	}
}
