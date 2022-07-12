namespace ImpossibleOdds.Examples.Http
{
	using System.Collections.Generic;
	using ImpossibleOdds.Http;

	[HttpBodyObject]
	public class Leaderboard
	{
		[HttpBodyField("Name")]
		private string name = string.Empty;
		[HttpBodyField("Entries")]
		private List<LeaderboardEntry> entries = null;

		public string Name
		{
			get => name;
		}

		public IReadOnlyList<LeaderboardEntry> Entries
		{
			get => entries;
		}
	}
}
