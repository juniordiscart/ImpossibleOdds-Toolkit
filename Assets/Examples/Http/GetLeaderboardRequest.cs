namespace ImpossibleOdds.Examples.Http
{
	using ImpossibleOdds.Http;

	[HttpObject, HttpResponseType(typeof(GetLeaderboardResponse))]
	public class GetLeaderboardRequest : IHttpPostRequest
	{
		[HttpBodyField("LeaderboardId")]
		private readonly string leaderboardID;
		[HttpBodyField("NrOfEntries")]
		private readonly int nrOfEntries;
		[HttpBodyField("Offset")]
		private readonly int offset;

		public string URL
		{
			get { return "https://impossible-odds.net/toolkit/examples/getleaderboard.php"; }
		}

		public GetLeaderboardRequest(string leaderboardID, int nrOfEntries, int offset)
		{
			this.leaderboardID = leaderboardID;
			this.nrOfEntries = nrOfEntries;
			this.offset = offset;
		}
	}
}
