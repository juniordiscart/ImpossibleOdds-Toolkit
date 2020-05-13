﻿namespace ImpossibleOdds.Serialization
{
	/// <summary>
	/// Type resolve interface for sequence-based data structures.
	/// </summary>
	public interface IIndexTypeResolveParameter : ISerializationTypeResolveParameter
	{
		/// <summary>
		/// The index at which the type information is stored in the index-based data structure.
		/// </summary>
		int Index
		{
			get;
		}

		/// <summary>
		/// The value that uniquely links to the target type in the index-based data structure.
		/// </summary>
		object Value
		{
			get;
		}
	}
}