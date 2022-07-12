namespace ImpossibleOdds.Examples.Xml
{
	using System;
	using System.Text;
	using UnityEngine;
	using ImpossibleOdds.Xml;

	[XmlObject]
	public class Actor
	{
		public static StringBuilder SerializationLog
		{
			get;
			set;
		}

		[XmlAttribute("Name"), XmlRequired(NullCheck = true)]
		private string name = string.Empty;
		[XmlElement("Biography")]
		private string bio = string.Empty;
		[XmlElement("DateOfBirth")]
		private DateTime dateOfBirth = DateTime.MinValue;
		[XmlCData("Picture")]
		private byte[] rawPictureData = null;

		private Texture2D picture = null;

		public string Name
		{
			get => name;
			set => name = value;
		}

		public string Biography
		{
			get => bio;
			set => bio = value;
		}

		public DateTime DateOfBirth
		{
			get => dateOfBirth;
			set => dateOfBirth = value;
		}

		private Texture2D Picture
		{
			get => picture;
			set => picture = value;
		}

		[OnXmlSerializing]
		private void OnSerializing()
		{
			SerializationLog.AppendLine(string.Format("Serializing actor with name {0}.", Name));
			rawPictureData = (picture != null) ? picture.EncodeToPNG() : null;
		}

		[OnXmlSerialized]
		private void OnSerialized()
		{
			SerializationLog.AppendLine(string.Format("Serialized actor with name {0}.", Name));
			rawPictureData = null;  // Clear the data again after serialization.
		}

		[OnXmlDeserializing]
		private void OnDeserializing()
		{
			SerializationLog.AppendLine(string.Format("Deserializing actor. No name available yet."));
			rawPictureData = null;  // Clear the data before deserialization.
		}

		[OnXmlDeserialized]
		private void OnDeserialized()
		{
			SerializationLog.AppendLine(string.Format("Deserialized actor with name {0}.", Name));

			if ((rawPictureData != null) && (rawPictureData.Length > 0))
			{
				picture = new Texture2D(2, 2);
				picture.LoadImage(rawPictureData);
				rawPictureData = null;  // Clear the data as we don't need it anymore now.
				SerializationLog.AppendLine(string.Format("Actor {0} has a profile picture of size {1}x{2}.", Name, picture.width, picture.height));
			}
		}
	}
}
