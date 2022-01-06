namespace ImpossibleOdds.Xml.Processors
{
	using ImpossibleOdds.Serialization.Processors;

	/// <summary>
	/// Processing methods for displaying Unity primitive types.
	/// </summary>
	public enum XmlPrimitiveProcessingMethod
	{
		/// Process fields from/to attributes on the XML element.
		ATTRIBUTES,
		/// Process fields from/to child elements of the XML element.
		ELEMENTS
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
