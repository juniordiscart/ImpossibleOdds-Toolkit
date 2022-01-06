namespace ImpossibleOdds.Serialization.Processors
{
	public enum PrimitiveProcessingMethod
	{
		SEQUENCE,
		LOOKUP
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
