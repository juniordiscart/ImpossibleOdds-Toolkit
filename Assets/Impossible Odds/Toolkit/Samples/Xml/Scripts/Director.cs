namespace ImpossibleOdds.Examples.Xml
{
	using System;
	using System.Text;
	using ImpossibleOdds.Xml;

	[XmlObject]
	public class Producer
	{
		public static StringBuilder SerializationLog
		{
			get;
			set;
		}

		[XmlAttribute("Name")]
		private string name = string.Empty;
		private DateTime dateOfBirth = DateTime.MinValue;

		public string Name
		{
			get => name;
			set => name = value;
		}

		[XmlElement("DateOfBirth")]
		public DateTime DateOfBirth
		{
			get => dateOfBirth;
			set => dateOfBirth = value;
		}

		[OnXmlSerializing]
		private void OnSerializing()
		{
			SerializationLog.AppendLine(string.Format("Serializing director with name {0}.", Name));
		}

		[OnXmlSerialized]
		private void OnSerialized()
		{
			SerializationLog.AppendLine(string.Format("Serialized director with name {0}.", Name));
		}

		[OnXmlDeserializing]
		private void OnDeserializing()
		{
			SerializationLog.AppendLine(string.Format("Deserializing director. No name available yet."));
		}

		[OnXmlDeserialized]
		private void OnDeserialized()
		{
			SerializationLog.AppendLine(string.Format("Deserialized director with name {0}.", Name));
		}
	}
}
