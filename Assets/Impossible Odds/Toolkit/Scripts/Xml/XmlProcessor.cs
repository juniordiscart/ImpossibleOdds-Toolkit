namespace ImpossibleOdds.Xml
{
	using System;
	using System.Xml;

	public static class XmlProcessor
	{
		public static string Serialize(object obj)
		{
			throw new NotImplementedException();
		}

		public static string Serialize(XmlDocument document)
		{
			document.ThrowIfNull(nameof(document));
			return document.OuterXml;
		}

		public static XmlDocument Deserialize(string xmlStr)
		{
			XmlDocument document = new XmlDocument();
			document.LoadXml(xmlStr);
			return document;
		}

		public static TTarget Deserialize<TTarget>(string xmlStr)
		{
			return (TTarget)Deserialize(typeof(TTarget), xmlStr);
		}

		public static object Deserialize(Type targetType, string xmlStr)
		{
			throw new NotImplementedException();
		}
	}
}
