using System;

namespace ImpossibleOdds.Serialization.Caching
{
	public class InvertedSequenceTypeResolutionParameter : InvertedTypeResolutionParameter, ISequenceTypeResolutionParameter
	{
		/// <inheritdoc />
		public int IndexOverride => ((ISequenceTypeResolutionParameter)OriginalParameter).IndexOverride; 
		
		public InvertedSequenceTypeResolutionParameter(Type target, ISequenceTypeResolutionParameter originalParameter)
		: base(target, originalParameter)
		{ }
	}
}