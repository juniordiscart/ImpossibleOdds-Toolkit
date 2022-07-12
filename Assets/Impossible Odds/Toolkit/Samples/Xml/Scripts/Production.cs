namespace ImpossibleOdds.Examples.Xml
{
	using System.Text;
	using UnityEngine;
	using ImpossibleOdds.Xml;

	[XmlType(typeof(Movie), KeyOverride = "ProductionType", Value = ProductionType.MOVIE, SetAsElement = true),
	XmlType(typeof(Series))]
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
		private Texture2D poster = null;

		[XmlCData("Poster")]
		private byte[] rawPosterData = null;

		[XmlAttribute, XmlRequired(NullCheck = true)]
		public string Name
		{
			get => name;
			set => name = value;
		}

		[XmlElement]
		public float Score
		{
			get => score;
			set => score = value;
		}

		[XmlElement]
		public Genre Genre
		{
			get => genre;
			set => genre = value;
		}

		[XmlElement]
		public string Director
		{
			get => director;
			set => director = value;
		}

		[XmlListElement(EntryName = "Actor")]
		public string[] Actors
		{
			get => actors;
			set => actors = value;
		}

		public Texture2D Poster
		{
			get => poster;
			set => poster = value;
		}

		[OnXmlSerializing]
		private void OnSerializing()
		{
			SerializationLog.AppendLine(string.Format("Serializing production of type {0} with name {1}.", this.GetType().Name, Name));
			rawPosterData = (poster != null) ? poster.EncodeToPNG() : null;
		}

		[OnXmlSerialized]
		private void OnSerialized()
		{
			SerializationLog.AppendLine(string.Format("Serialized production of type {0} with name {1}.", this.GetType().Name, Name));
			rawPosterData = null;   // Clear the data again after seriaization.
		}

		[OnXmlDeserializing]
		private void OnDeserializing()
		{
			SerializationLog.AppendLine(string.Format("Deserializing production of type {0}. No name is available yet.", this.GetType().Name));
			rawPosterData = null;   // Clear the data before deserialization.
		}

		[OnXmlDeserialized]
		private void OnDeserialized()
		{
			SerializationLog.AppendLine(string.Format("Deserialized production of type {0} with name {1}.", this.GetType().Name, Name));

			if ((rawPosterData != null) && (rawPosterData.Length > 0))
			{
				poster = new Texture2D(2, 2);
				poster.LoadImage(rawPosterData);
				rawPosterData = null;  // Clear the data as we don't need it anymore now.
				SerializationLog.AppendLine(string.Format("Production {0} has a poster of size {1}x{2}.", Name, poster.width, poster.height));
			}
		}
	}
}
