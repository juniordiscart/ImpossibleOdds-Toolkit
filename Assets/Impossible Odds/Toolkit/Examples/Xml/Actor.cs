namespace ImpossibleOdds.Examples.Xml
{
	using System;
	using System.Text;
	using ImpossibleOdds.Xml;

	[XmlObject, Serializable]
	public class Actor
	{
		public static StringBuilder SerializationLog
		{
			get;
			set;
		}

		[XmlAttribute("Name")]
		private string name = string.Empty;
		[XmlElement("Biography")]
		private string bio = string.Empty;
		[XmlElement("DateOfBirth")]
		private DateTime dateOfBirth = DateTime.MinValue;

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public string Biography
		{
			get { return bio; }
			set { bio = value; }
		}

		public DateTime DateOfBirth
		{
			get { return dateOfBirth; }
			set { dateOfBirth = value; }
		}

		[OnXmlSerializing]
		private void OnSerializing()
		{
			SerializationLog.AppendLine(string.Format("Serializing actor with name {0}.", Name));
		}

		[OnXmlSerialized]
		private void OnSerialized()
		{
			SerializationLog.AppendLine(string.Format("Serialized actor with name {0}.", Name));
		}

		[OnXmlDeserializing]
		private void OnDeserializing()
		{
			SerializationLog.AppendLine(string.Format("Deserializing actor. No name available yet."));
		}

		[OnXmlDeserialized]
		private void OnDeserialized()
		{
			SerializationLog.AppendLine(string.Format("Deserialized actor with name {0}.", Name));
		}
	}
}
