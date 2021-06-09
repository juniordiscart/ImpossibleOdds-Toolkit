namespace ImpossibleOdds.Examples.Xml
{
	using System.Text;
	using ImpossibleOdds.Xml;

	[XmlType(typeof(Movie), Value = "Movie"),
	XmlType(typeof(Series), Value = "Series")]
	public abstract class Production
	{
		public static StringBuilder SerializationLog
		{
			get;
			set;
		}

		private string name = string.Empty;

		private float score = 0f;
		private Genre genre = Genre.UNKNOWN;
		private string director = string.Empty;
		private string[] actors = null;

		[XmlAttribute, XmlRequired]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[XmlElement]
		public float Score
		{
			get { return score; }
			set { score = value; }
		}

		[XmlElement]
		public Genre Genre
		{
			get { return genre; }
			set { genre = value; }
		}

		[XmlElement]
		public string Director
		{
			get { return director; }
			set { director = value; }
		}

		[XmlListElement(EntryName = "Actor")]
		public string[] Actors
		{
			get { return actors; }
			set { actors = value; }
		}

		[OnXmlSerializing]
		private void OnSerializing()
		{
			SerializationLog.AppendLine(string.Format("Serializing production of type {0} with name {1}.", this.GetType().Name, Name));
		}

		[OnXmlSerialized]
		private void OnSerialized()
		{
			SerializationLog.AppendLine(string.Format("Serialized production of type {0} with name {1}.", this.GetType().Name, Name));
		}

		[OnXmlDeserializing]
		private void OnDeserializing()
		{
			SerializationLog.AppendLine(string.Format("Deserializing production of type {0}. No name is available yet.", this.GetType().Name));
		}

		[OnXmlDeserialized]
		private void OnDeserialized()
		{
			SerializationLog.AppendLine(string.Format("Deserialized production of type {0} with name {1}.", this.GetType().Name, Name));
		}
	}
}
