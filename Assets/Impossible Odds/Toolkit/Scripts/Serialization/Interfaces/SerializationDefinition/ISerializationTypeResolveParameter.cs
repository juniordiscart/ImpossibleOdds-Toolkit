namespace ImpossibleOdds.Serialization
{
	using System;

	/// <summary>
	/// Base interface for type resolvement interfaces.
	/// </summary>
	public interface ISerializationTypeResolveParameter
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
