#if PHOTON_UNITY_NETWORKING
#define PHOTON_AVAILABLE
#endif

#if PHOTON_AVAILABLE
namespace ImpossibleOdds.Examples.Photon.WebRpc
{
	using System.Text;
	using ImpossibleOdds.Photon.WebRpc;

	[WebRpcResponseType(typeof(UpdateLeaderboardResponse)), WebRpcObject]
	public class UpdateLeaderboardRequest : IWebRpcRequest
	{
		[WebRpcField("UserId")]
		private int userID;
		[WebRpcField("LeaderboardId")]
		private string leaderboardID;
		[WebRpcField("Score")]
		private int score;
		[WebRpcField("ForceUpdate")]
		private bool forceUpdate;

		/// <inheritdoc />
		public string UriPath
		{
			get => "webrpc/updateleaderboard.php";
		}

		/// <inheritdoc />
		public bool UseAuthCookie
		{
			get => false;
		}

		/// <inheritdoc />
		public bool UseEncryption
		{
			get => true;
		}

		public UpdateLeaderboardRequest(int userID, string leaderboardID, int score, bool forceUpdate = false)
		{
			leaderboardID.ThrowIfNullOrWhitespace(leaderboardID);

			this.userID = userID;
			this.leaderboardID = leaderboardID;
			this.score = score;
			this.forceUpdate = forceUpdate;
		}

		public void ToString(StringBuilder builder)
		{
			builder.AppendFormat("User ID: {0}\n", userID);
			builder.AppendFormat("Leaderboard ID: {0}\n", leaderboardID);
			builder.AppendFormat("Score: {0}\n", score);
			builder.AppendFormat("Force update: {0}\n", forceUpdate);
		}
	}

	[WebRpcObject]
	public class UpdateLeaderboardResponse : IWebRpcResponse
	{
		[WebRpcField("ErrorCode")]
		private ResultCode resultCode = ResultCode.None;
		[WebRpcField("UpdateCode")]
		private UpdateCode updateCode = UpdateCode.None;
		[WebRpcField("CurrentScore")]
		private int currentScore;

		public bool IsSuccess
		{
			get => (resultCode == ResultCode.None) && (updateCode != UpdateCode.None);
		}

		public ResultCode ResultCode
		{
			get => resultCode;
		}

		public UpdateCode UpdateCode
		{
			get => updateCode;
		}

		public int CurrentScore
		{
			get => currentScore;
		}

		public void ToString(StringBuilder builder)
		{
			builder.AppendFormat("Success: {0}\n", IsSuccess);
			builder.AppendFormat("Result code: {0}\n", resultCode.DisplayName());
			builder.AppendFormat("Update code: {0}\n", updateCode.DisplayName());
			builder.AppendFormat("Current score: {0}\n", currentScore);
		}
	}

	public enum ResultCode
	{
		[DisplayName(Name = "No error")]
		None = 0,
		[DisplayName(Name = "Invalid leaderboard ID")]
		InvalidLeaderboardID = 1,
		[DisplayName(Name = "Invalid score")]
		InvalidScore = 2
	}

	public enum UpdateCode
	{
		[DisplayName]
		None = 0,
		[DisplayName(Name = "Score updated")]
		ScoreUpdated = 1,
		[DisplayName(Name = "Score unchanged")]
		ScoreUnchanged = 2
	}
}
#endif
