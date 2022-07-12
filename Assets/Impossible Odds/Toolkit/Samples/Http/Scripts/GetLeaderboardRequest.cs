namespace ImpossibleOdds.Examples.Http
{
	using System.Text;
	using ImpossibleOdds.Http;

	[HttpBodyObject, HttpResponseType(typeof(GetLeaderboardResponse))]
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
			get => "https://impossible-odds.net/toolkit/examples/getleaderboard.php";
		}

		public GetLeaderboardRequest(string leaderboardID, int nrOfEntries, int offset)
		{
			this.leaderboardID = leaderboardID;
			this.nrOfEntries = nrOfEntries;
			this.offset = offset;
		}

		public void ToString(StringBuilder builder)
		{
			builder.AppendFormat("Leaderboard ID: {0}\n", leaderboardID);
			builder.AppendFormat("Number of entries: {0}\n", nrOfEntries);
			builder.AppendFormat("Offset in ranking: {0}\n", offset);
		}
	}
}
