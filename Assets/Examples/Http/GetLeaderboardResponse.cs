namespace ImpossibleOdds.Examples.Http
{
	using System;
	using ImpossibleOdds.Http;

	[Flags]
	public enum ResponseError
	{
		NONE = 0,
		INVALID_ID = 1,
		INVALID_ENTRIES = 1 << 1,
		INVALID_OFFSET = 1 << 2
	}

	[HttpObject]
	public class GetLeaderboardResponse : IHttpJsonResponseHandler
	{
		[HttpBodyField("ErrorCode")]
		private ResponseError responseError = 0;
		[HttpBodyField("Leaderboard")]
		private Leaderboard leaderboard = null;

		public bool IsSuccess
		{
			get { return (responseError == ResponseError.NONE) && (leaderboard != null); }
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
