using System;

namespace ImpossibleOdds.Serialization.Caching
{
	public class InvertedTypeResolutionParameter : IInvertedTypeResolutionParameter
	{
		/// <inheritdoc />
		public Type Target { get; }

		/// <inheritdoc />
		public object Value => OriginalParameter.Value;

		/// <inheritdoc />
		public ITypeResolutionParameter OriginalParameter
		{
			get;
		}

		public InvertedTypeResolutionParameter(Type target, ITypeResolutionParameter originalParameter)
		{
			target.ThrowIfNull(nameof(target));
			originalParameter.ThrowIfNull(nameof(originalParameter));
			
			Target = target;
			OriginalParameter = originalParameter;
		}
	}
}