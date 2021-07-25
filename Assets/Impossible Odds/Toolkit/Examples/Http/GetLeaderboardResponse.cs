namespace ImpossibleOdds.Examples.Http
{
	using System;
	using ImpossibleOdds.Http;

	[Flags]
	public enum ResponseError
	{
		None = 0,
		InvalidID = 1,
		InvalidEntries = 1 << 1,
		InvalidOffset = 1 << 2
	}

	[HttpBodyObject]
	public class GetLeaderboardResponse : IHttpJsonResponse
	{
		[HttpBodyField("ErrorCode")]
		private ResponseError responseError = 0;
		[HttpBodyField("Leaderboard")]
		private Leaderboard leaderboard = null;

		public bool IsSuccess
		{
			get { return (responseError == ResponseError.None) && (leaderboard != null); }
		}

		public ResponseError ResponseError
		{
			get { return responseError; }
		}

		public Leaderboard Leaderboard
		{
			get { return leaderboard; }
		}
	}
}
