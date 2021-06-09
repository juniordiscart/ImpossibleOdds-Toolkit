namespace ImpossibleOdds.Examples.Xml
{
	using System.Collections.Generic;
	using System.Text;
	using ImpossibleOdds.Xml;

	[XmlObject(RootName = "IMDB")]
	public class MovieDatabase
	{
		public static StringBuilder SerializationLog
		{
			get;
			set;
		}

		[XmlListElement("Actors", EntryName = "Actor")]
		private List<Actor> actors = new List<Actor>();
		[XmlListElement("Producers", EntryName = "Producer")]
		private List<Producer> producers = new List<Producer>();
		[XmlListElement("Productions", EntryName = "Production")]
		private List<Production> productions = new List<Production>();
		[XmlCData("Logo")]
		private byte[] logo = null;

		public List<Actor> Actors { get => actors; set => actors = value; }
		public List<Producer> Producers { get => producers; set => producers = value; }
		public List<Production> Productions { get => productions; set => productions = value; }
		public byte[] Logo { get => logo; set => logo = value; }

		[OnXmlSerializing]
		private void OnSerializing()
		{
			SerializationLog.AppendLine("Serializing the movie database.");
		}

		[OnXmlSerialized]
		private void OnSerialized()
		{
			SerializationLog.AppendLine("Serialized the movie database.");
		}

		[OnXmlDeserializing]
		private void OnDeserializing()
		{
			SerializationLog.AppendLine("Deserializing the movie database.");
		}

		[OnXmlDeserialized]
		private void OnDeserialized()
		{
			SerializationLog.AppendLine("Deserialized the movie database.");
		}
	}
}
