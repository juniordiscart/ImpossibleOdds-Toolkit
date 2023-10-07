using ImpossibleOdds.Serialization.Processors;

namespace ImpossibleOdds.Xml.Processors
{
	/// <summary>
	/// Processing methods for displaying Unity primitive types.
	/// </summary>
	public enum XmlPrimitiveProcessingMethod
	{
		Attributes,
		Elements
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