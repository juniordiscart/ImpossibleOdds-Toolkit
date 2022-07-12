namespace ImpossibleOdds.Examples.Http
{
	using System;
	using System.Text;
	using ImpossibleOdds.Http;

	[Flags]
	public enum ResponseError
	{
		[DisplayName(Name = "No error")]
		None = 0,
		[DisplayName(Name = "Invalid Leaderboard ID")]
		InvalidID = 1,
		[DisplayName(Name = "Invalid range of entries")]
		InvalidEntries = 1 << 1,
		[DisplayName(Name = "Invalid offset")]
		InvalidOffset = 1 << 2
	}

	[HttpBodyObject]
	public class GetLeaderboardResponse : IHttpStructuredResponse
	{
		[HttpBodyField("ErrorCode")]
		private ResponseError responseError = 0;
		[HttpBodyField("Leaderboard")]
		private Leaderboard leaderboard = null;

		public bool IsSuccess
		{
			get => (responseError == ResponseError.None) && (leaderboard != null);
		}

		public ResponseError ResponseError
		{
			get => responseError;
		}

		public Leaderboard Leaderboard
		{
			get => leaderboard;
		}

		public void ToString(StringBuilder builder)
		{
			builder.AppendFormat("Success: {0}\n", IsSuccess);
			builder.AppendFormat("Error: {0}\n", responseError.DisplayName());

			if (leaderboard != null)
			{
				builder.AppendFormat("Leaderboard: {0}\n", leaderboard.Name);
				builder.AppendFormat("Entries: {0}\n", leaderboard.Entries.Count);

				foreach (LeaderboardEntry entry in leaderboard.Entries)
				{
					builder.AppendFormat("\t{0}. - User ID: {1} - Score: {2}\n", (entry.Rank + 1), entry.PlayerId, entry.Score);
				}
			}
		}
	}
}
