using System;

namespace ImpossibleOdds.Serialization.Caching
{
	public class InvertedLookupTypeResolutionParameter : InvertedTypeResolutionParameter, ILookupTypeResolutionParameter
	{
		/// <inheritdoc />
		public object KeyOverride => ((ILookupTypeResolutionParameter)OriginalParameter).KeyOverride;

		public InvertedLookupTypeResolutionParameter(Type target, ILookupTypeResolutionParameter originalParameter)
		: base(target, originalParameter)
		{ }
	}
}