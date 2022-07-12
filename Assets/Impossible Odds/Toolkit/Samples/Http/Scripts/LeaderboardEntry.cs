namespace ImpossibleOdds.Examples.Http
{
	using ImpossibleOdds.Http;

	[HttpBodyArray]
	public class LeaderboardEntry
	{
		[HttpBodyIndex(0)]
		private int rank = 0;
		[HttpBodyIndex(1)]
		private int playerId = 0;
		[HttpBodyIndex(2)]
		private int score = 0;

		public int Rank
		{
			get => rank;
		}

		public int PlayerId
		{
			get => playerId;
		}

		public int Score
		{
			get => score;
		}
	}
}
