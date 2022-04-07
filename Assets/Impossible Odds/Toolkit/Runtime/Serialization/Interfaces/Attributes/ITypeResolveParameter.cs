namespace ImpossibleOdds.Serialization
{
	using System;

	/// <summary>
	/// Base interface for type resolve interfaces.
	/// </summary>
	public interface ITypeResolveParameter
	{
		/// <summary>
		/// The target type defined for the type resolve.
		/// </summary>
		Type Target
		{
			get;
		}

		/// <summary>
		/// Optional alias for the target type. This must be unique within the scope of types it is defined for.
		/// </summary>
		object Value
		{
			get;
		}
	}
}
