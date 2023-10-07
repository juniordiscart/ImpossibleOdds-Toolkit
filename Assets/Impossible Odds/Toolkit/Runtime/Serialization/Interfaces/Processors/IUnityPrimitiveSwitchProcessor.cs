namespace ImpossibleOdds.Serialization.Processors
{
	public enum PrimitiveProcessingMethod
	{
		Sequence,
		Lookup,
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