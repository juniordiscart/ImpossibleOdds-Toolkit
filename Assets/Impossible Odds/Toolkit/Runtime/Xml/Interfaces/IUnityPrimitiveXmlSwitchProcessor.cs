namespace ImpossibleOdds.Xml.Processors
{
	using ImpossibleOdds.Serialization.Processors;

	/// <summary>
	/// Processing methods for displaying Unity primitive types.
	/// </summary>
	public enum XmlPrimitiveProcessingMethod
	{
#pragma warning disable 0618
		[System.Obsolete("Use `Attributes` instead.")]
		ATTRIBUTES,
		Attributes = ATTRIBUTES,
		[System.Obsolete("Use `Elements` instead")]
		ELEMENTS,
		Elements = ELEMENTS
#pragma warning restore 0618
	}

	/// <summary>
	/// Basic interface for defining an XML Unity primitive processor that is able to switch between processing methods.
	/// </summary>
	public interface IUnityPrimitiveXmlSwitchProcessor : IProcessor
	{
		/// <summary>
		/// The preferred processing method of this processor.
		/// </summary>
		XmlPrimitiveProcessingMethod ProcessingMethod
		{
			get;
			set;
		}
	}
}
