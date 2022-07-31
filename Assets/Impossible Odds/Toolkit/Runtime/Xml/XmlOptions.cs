namespace ImpossibleOdds.Xml
{
	using System.Text;
	using System.Xml;

	public class XmlOptions
	{
		/// <summary>
		/// Enable/disable whether the output should waste as little space as possible.
		/// </summary>
		public bool CompactOutput
		{
			get;
			set;
		}

		/// <summary>
		/// Hide/show the XML header when generating output.
		/// </summary>
		public bool HideHeader
		{
			get;
			set;
		}

		/// <summary>
		/// The reported encoding in the header of the XML document.
		/// Note: the applied encoding partially depends on the underlying text writer object whether it will be applied or not.
		/// </summary>
		public Encoding Encoding
		{
			get;
			set;
		}

		/// <summary>
		/// A customized serialization definition for processing XML elements.
		/// </summary>
		public XmlSerializationDefinition SerializationDefinition
		{
			get;
			set;
		}

		/// <summary>
		/// Settings for the XML reader object.
		/// </summary>
		public XmlReaderSettings ReaderSettings
		{
			get;
			set;
		}

		public XmlOptions()
		{
			CompactOutput = true;
			HideHeader = false;
			Encoding = Encoding.UTF8;
			SerializationDefinition = null;
		}
	}
}
