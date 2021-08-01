namespace ImpossibleOdds.Examples.Http
{
	using ImpossibleOdds.Http;

	[HttpBodyArray]
	public class LeaderboardEntry
	{
		[HttpBodyIndex(0)]
		private int rank = 0;
		[HttpBodyIndex(1)]
		private int playerID = 0;
		[HttpBodyIndex(2)]
		private int score = 0;

		public int Rank
		{
			get { return rank; }
		}

		public int PlayerID
		{
			get { return playerID; }
		}

		public int Score
		{
			get { return score; }
		}
	}
}
