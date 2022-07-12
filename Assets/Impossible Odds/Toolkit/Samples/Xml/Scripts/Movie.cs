namespace ImpossibleOdds.Examples.Xml
{
	using ImpossibleOdds.Xml;

	[XmlObject]
	public class Movie : Production
	{
		private int releaseYear = 0;

		[XmlElement]
		public int ReleaseYear
		{
			get => releaseYear;
			set => releaseYear = value;
		}
	}
}
