namespace ImpossibleOdds.Xml
{
	using System.Xml;

	internal struct DeserializationState
	{
		public readonly XmlSerializationDefinition definition;
		public readonly XmlReader reader;

		public XmlNodeType NodeType
		{
			get => reader.NodeType;
		}

		public string Name
		{
			get => reader.Name;
		}

		public string Value
		{
			get => reader.Value;
		}

		public bool HasAttributes
		{
			get => reader.HasAttributes;
		}

		public bool IsEmptyElement
		{
			get => reader.IsEmptyElement;
		}

		public bool Read()
		{
			return reader.Read();
		}

		public bool MoveToAttribute(string attributeName)
		{
			return reader.MoveToAttribute(attributeName);
		}

		public bool MoveToNextAttribute()
		{
			return reader.MoveToNextAttribute();
		}

		public bool MoveToElement()
		{
			return reader.MoveToElement();
		}
		
		public string GetAttribute(string attributeName)
		{
			return reader.GetAttribute(attributeName);
		}

		public DeserializationState(XmlSerializationDefinition definition, XmlReader reader)
		{
			this.definition = definition;
			this.reader = reader;
		}
	}
}
