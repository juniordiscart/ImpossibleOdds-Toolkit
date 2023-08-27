using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using ImpossibleOdds;
using ImpossibleOdds.Xml;
using UnityEngine;

public class XmlParsing : MonoBehaviour
{
	[SerializeField]
	private TextAsset xmlTextAsset = null;

	private void Start()
	{
		XmlReaderSettings readerSettings = new XmlReaderSettings();
		readerSettings.DtdProcessing = DtdProcessing.Parse;

		XmlWriterSettings writerSettings = new XmlWriterSettings();
		writerSettings.Indent = true;

		XDocument document = null;

		using (StringReader textReader = new StringReader(xmlTextAsset.text))
		using (XmlReader xmlReader = XmlReader.Create(textReader, readerSettings))
		{
			// DebugPrintXml(xmlReader);
			// document = XmlProcessor.FromXml(xmlReader);
		}

		StringBuilder textBuilder = new StringBuilder();
		using (StringWriter textWriter = new StringWriter(textBuilder))
		using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, writerSettings))
		{
			document.WriteTo(xmlWriter);
		}

		Log.Info(textBuilder.ToString());
	}

	private void DebugPrintXml(XmlReader xmlReader)
	{
		while (xmlReader.Read())
		{
			switch (xmlReader.NodeType)
			{
				case XmlNodeType.Element:
				case XmlNodeType.EndElement:
					Log.Info("Reading node of type {0} with name {1}.", xmlReader.NodeType.DisplayName(), xmlReader.Name);
					break;
				default:
					// Log.Info("Reading node of type {0} at depth {1}.", xmlReader.NodeType.DisplayName(), xmlReader.Depth);
					break;
			}

			if (xmlReader.HasAttributes)
			{
				Log.Info("Reading attributes of {0}. ", xmlReader.Name);
				while (xmlReader.MoveToNextAttribute())
				{
					Log.Info("Reading attribute {0} with value '{1}'.", xmlReader.Name, xmlReader.Value);
				}
				xmlReader.MoveToElement();
			}
		}
	}
}
