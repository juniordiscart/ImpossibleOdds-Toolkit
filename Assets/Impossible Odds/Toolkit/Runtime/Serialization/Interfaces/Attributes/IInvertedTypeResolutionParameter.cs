using System;

namespace ImpossibleOdds.Serialization
{
	/// <summary>
	/// Interface for type resolution parameters that are used to refer back to base-types.
	/// </summary>
	public interface IInvertedTypeResolutionParameter : ITypeResolutionParameter
	{
		/// <summary>
		/// The original parameter associated with the type resolution attribute.
		/// </summary>
		ITypeResolutionParameter OriginalParameter
		{
			get;
		}
	}
}