namespace ImpossibleOdds.Serialization.Processors
{
	public enum PrimitiveProcessingMethod
	{
		#pragma warning disable 0618
		[System.Obsolete("Use `Sequence` instead.")]
		SEQUENCE,
		Sequence = SEQUENCE,
		[System.Obsolete("Use `LookUp` instead.")]
		LOOKUP,
		Lookup = LOOKUP
		#pragma warning restore 0618
	}

	public interface IUnityPrimitiveSwitchProcessor : IProcessor
	{
		PrimitiveProcessingMethod ProcessingMethod
		{
			get;
			set;
		}
	}
}
