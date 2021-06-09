namespace ImpossibleOdds.Examples.Xml
{
	using System;
	using System.Text;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using ImpossibleOdds.Xml;
	using TMPro;
	using System.IO;

	public class TestXmlSerialization : MonoBehaviour
	{
		[SerializeField]
		private Sprite logo = null;
		[SerializeField]
		private Button btnSerialize = null;
		[SerializeField]
		private Button btnDeserialize = null;
		[SerializeField]
		private Button btnCopyToClipboard = null;
		[SerializeField]
		private TMP_InputField txtXml = null;
		[SerializeField]
		private TextMeshProUGUI txtLog = null;

		private MovieDatabase movieDB = null;
		private XmlOptions options = null;

		private StringBuilder xmlBuilder = null;
		private StringBuilder logBuilder = null;

		private void Awake()
		{
			xmlBuilder = new StringBuilder();
			logBuilder = new StringBuilder();
			options = new XmlOptions()
			{
				CompactOutput = false,
				HideHeader = false,
				Encoding = Encoding.UTF8,
			};

			Actor.SerializationLog = logBuilder;
			Producer.SerializationLog = logBuilder;
			Production.SerializationLog = logBuilder;
			MovieDatabase.SerializationLog = logBuilder;
		}

		private void Start()
		{
			Application.targetFrameRate = 60;

			btnSerialize.onClick.AddListener(OnSerialize);
			btnDeserialize.onClick.AddListener(OnDeserialize);
			btnCopyToClipboard.onClick.AddListener(CopySerializedResultToClipboard);
			btnDeserialize.interactable = false;
			txtXml.text = string.Empty;
			txtLog.text = string.Empty;
		}

		private void OnSerialize()
		{
			List<Actor> actors = new List<Actor>()
			{
				new Actor()
				{
					Name = "Bob Odenkirk",
					DateOfBirth = new DateTime(1962, 10, 22),
					Biography = "Robert John Odenkirk was born in Berwyn, Illinois, to Barbara (Baier) and Walter Odenkirk, who worked in printing."
				},
				new Actor()
				{
					Name = "Giancarlo Esposito",
					DateOfBirth = new DateTime(1958, 4, 26),
					Biography = "Giancarlo Giuseppe Alessandro Esposito was born in Copenhagen, Denmark, to an Italian carpenter/stagehand father from Naples, Italy, and an African-American opera singer mother from Alabama."
				},
				new Actor()
				{
					Name = "Brian Cranston",
					DateOfBirth = new DateTime(1956, 3, 7),
					Biography= "Bryan Lee Cranston was born on March 7, 1956 in Hollywood, California, to Audrey Peggy Sell, a radio actress, and Joe Cranston, an actor and former amateur boxer."
				},
				new Actor()
				{
					Name = "Christian Bale",
					DateOfBirth = new DateTime(1974, 1, 30),
					Biography = "Christian Charles Philip Bale was born in Pembrokeshire, Wales, UK on January 30, 1974, to English parents Jennifer \"Jenny\" (James) and David Bale."
				},
				new Actor()
				{
					Name = "Heath Ledger",
					DateOfBirth = new DateTime(1979, 4, 4),
					Biography = "When hunky, twenty-year-old heart-throb Heath Ledger first came to the attention of the public in 1999, it was all too easy to tag him as a \"pretty boy\" and an actor of little depth."
				},
			};

			List<Producer> producers = new List<Producer>()
			{
				new Producer()
				{
					 Name = "Christopher Nolan",
					 DateOfBirth = new DateTime(1970, 7, 30)
				},
				new Producer()
				{
					Name = "Vince Gilligan",
					DateOfBirth = new DateTime(1967, 2, 10)
				},
			};

			List<Production> productions = new List<Production>()
			{
				new Series()
				{
					Name = "Breaking Bad",
					Score = 9.4f,
					Genre = Genre.DRAMA | Genre.THRILLER,
					NrOfEpisodes = 62,
					NrOfSeasons = 5,
					StartedOn = new DateTime(2008, 1, 1),
					EndedOn = new DateTime(2013, 1, 1),
					Director = "Vince Gilligan",
					Actors = new string[] {"Brian Cranston", "Anna Gun", "Giancarlo Esposito", "Bob Odenkirk"}
				},
				new Series()
				{
					Name = "Better Call Saul",
					Score = 8.7f,
					Genre = Genre.DRAMA,
					NrOfEpisodes = 63,
					NrOfSeasons = 6,
					StartedOn = new DateTime(2015, 1, 1),
					Director = "Vince Gilligan",
					Actors = new string[] {"Bob Odenkirk", "Rhea Seehorn", "Jonathan Banks"}
				},
				new Movie()
				{
					Name = "The Dark Knight",
					Score = 9.0f,
					Genre = Genre.ACTION | Genre.THRILLER | Genre.DRAMA,
					Director = "Christopher Nolan",
					ReleaseYear = 2008,
					Actors = new string[] {"Christian Bale", "Heath Ledger", "Gary Oldman"}
				},
				new Movie()
				{
					Name = "Green Book",
					Score = 8.2f,
					Genre = Genre.COMEDY | Genre.DRAMA,
					ReleaseYear = 2018,
					Director = "Peter Farrelly",
					Actors = new string[] {"Viggo Mortensen", "Mahershala Ali", "Linda Cardellini"}
				}
			};

			movieDB = new MovieDatabase()
			{
				Logo = logo.texture.GetRawTextureData(),
				Actors = actors,
				Producers = producers,
				Productions = productions
			};

			xmlBuilder.Clear();
			logBuilder.Clear();

			XmlProcessor.Serialize(movieDB, options, new StringWriter(xmlBuilder));
			btnDeserialize.interactable = (xmlBuilder.Length > 0);

			txtLog.text = logBuilder.ToString();
			txtXml.text = xmlBuilder.ToString();
		}

		private void OnDeserialize()
		{
			MovieDatabase movieDatabase = XmlProcessor.Deserialize<MovieDatabase>(txtXml.text);
			txtLog.text = logBuilder.ToString();
		}

		private void CopySerializedResultToClipboard()
		{
			GUIUtility.systemCopyBuffer = xmlBuilder.ToString();
		}
	}
}
