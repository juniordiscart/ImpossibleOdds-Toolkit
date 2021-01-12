namespace ImpossibleOdds.Examples.Http
{
	using System.Collections.Generic;
	using ImpossibleOdds.Http;
	using ImpossibleOdds.Json;

	[HttpBodyObject, JsonObject]
	public class Leaderboard
	{
		[HttpBodyField("Name"), JsonField]
		private string name = string.Empty;
		[HttpBodyField("Entries"), JsonField]
		private List<LeaderboardEntry> entries = null;

		public string Name
		{
			get { return name; }
		}

		public IReadOnlyList<LeaderboardEntry> Entries
		{
			get { return entries; }
		}
	}
}
