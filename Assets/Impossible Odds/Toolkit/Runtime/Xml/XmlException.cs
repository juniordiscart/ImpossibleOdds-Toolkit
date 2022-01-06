namespace ImpossibleOdds.Xml
{
	public sealed class XmlException : ImpossibleOddsException
	{
		public XmlException()
		{ }

		public XmlException(string errMsg)
		: base(errMsg)
		{ }

		public XmlException(string errMsg, params object[] format)
		: base(string.Format(errMsg, format))
		{ }
	}
}
