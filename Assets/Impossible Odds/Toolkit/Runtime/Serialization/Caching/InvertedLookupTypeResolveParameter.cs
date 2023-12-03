using System;

namespace ImpossibleOdds.Serialization.Caching
{
	public class InvertedLookupTypeResolveParameter : ILookupTypeResolveParameter
	{
		/// <inheritdoc />
		public Type Target { get; internal set; }

		/// <inheritdoc />
		public object Value { get; internal set; }

		/// <inheritdoc />
		public object KeyOverride { get; internal set; }

		public ILookupTypeResolveParameter OriginalAttribute
		{
			get;
			internal set;
		}
	}
}