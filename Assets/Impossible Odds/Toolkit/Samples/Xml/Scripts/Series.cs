namespace ImpossibleOdds.Examples.Xml
{
	using System;
	using ImpossibleOdds.Xml;

	[XmlObject]
	public class Series : Production
	{
		private int nrOfEpisodes = 0;
		private int nrOfSeasons = 0;
		private DateTime runningSince = DateTime.MinValue;
		private DateTime endedOn = DateTime.MinValue;

		public bool IsRunning
		{
			get => endedOn == DateTime.MinValue;
		}

		[XmlElement]
		public int NrOfEpisodes
		{
			get => nrOfEpisodes;
			set => nrOfEpisodes = value;
		}

		[XmlElement]
		public int NrOfSeasons
		{
			get => nrOfSeasons;
			set => nrOfSeasons = value;
		}

		[XmlElement]
		public DateTime StartedOn
		{
			get => runningSince;
			set => runningSince = value;
		}

		[XmlElement]
		public DateTime EndedOn
		{
			get => endedOn;
			set => endedOn = value;
		}

	}
}
