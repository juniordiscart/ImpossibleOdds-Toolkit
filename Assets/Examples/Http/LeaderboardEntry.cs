namespace ImpossibleOdds.Examples.Http
{
	using ImpossibleOdds.Http;
	using ImpossibleOdds.Json;

	[HttpList, JsonObject]
	public class LeaderboardEntry
	{
		[HttpBodyIndex(0), JsonField]
		private int rank = 0;
		[HttpBodyIndex(1), JsonField]
		private int playerID = 0;
		[HttpBodyIndex(2), JsonField]
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
