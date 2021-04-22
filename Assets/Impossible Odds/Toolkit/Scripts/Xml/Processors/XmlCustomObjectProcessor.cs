namespace ImpossibleOdds.Xml
{
	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Serialization.Processors;

	public class XmlCustomObjectProcessor : CustomObjectLookupProcessor
	{
		public XmlCustomObjectProcessor(ILookupSerializationDefinition definition) : base(definition)
		{
		}
	}
}
