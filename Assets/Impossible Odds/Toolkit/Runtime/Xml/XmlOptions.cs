using System.Text;
using System.Xml;

namespace ImpossibleOdds.Xml
{
	public class XmlOptions
	{
		/// <summary>
		/// Enable/disable whether the output should waste as little space as possible.
		/// </summary>
		public bool CompactOutput
		{
			get;
			set;
		} = true;

		/// <summary>
		/// Hide/show the XML header when generating output.
		/// </summary>
		public bool HideHeader
		{
			get;
			set;
		} = false;

		/// <summary>
		/// The reported encoding in the header of the XML document.
		/// Note: the applied encoding partially depends on the underlying text writer object whether it will be applied or not.
		/// </summary>
		public Encoding Encoding
		{
			get;
			set;
		} = Encoding.UTF8;

		/// <summary>
		/// A customized serialization definition for processing XML elements.
		/// </summary>
		public XmlSerializationDefinition SerializationDefinition
		{
			get;
			set;
		} = null;

		/// <summary>
		/// Settings for the XML reader object.
		/// </summary>
		public XmlReaderSettings ReaderSettings
		{
			get;
			set;
		}
	}
}