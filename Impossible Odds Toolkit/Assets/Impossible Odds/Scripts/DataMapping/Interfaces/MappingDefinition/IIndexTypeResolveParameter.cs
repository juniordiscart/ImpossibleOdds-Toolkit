namespace ImpossibleOdds.DataMapping
{
	using System;

	/// <summary>
	/// Type resolve interface for sequence-based data structures.
	/// </summary>
	public interface IIndexTypeResolveParameter
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
		/// The index at which the type information is stored in the index-based data structure.
		/// </summary>
		/// <value></value>
		int Index
		{
			get;
		}

		/// <summary>
		/// The value that uniquely links to the target type in the index-based data structure.
		/// </summary>
		/// <value></value>
		object Value
		{
			get;
		}
	}
}
