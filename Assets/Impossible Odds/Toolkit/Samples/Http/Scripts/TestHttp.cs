namespace ImpossibleOdds.Examples.Http
{
	using System.Text;
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;

	using ImpossibleOdds.Http;
	using ImpossibleOdds.Json;

	public class TestHttp : MonoBehaviour
	{
		[SerializeField]
		private Button btnSendRequest = null;
		[SerializeField]
		private Button btnClearLog = null;

		[SerializeField]
		private TextMeshProUGUI txtLog = null;

		private HttpMessenger messenger = null;
		private JsonOptions jsonPrintOptions = null;
		private StringBuilder logBuilder = null;

		private void Awake()
		{
			// Messenger taking care of processing outgoing and incoming data
			// as well as setting up and managing the HTTP requests
			messenger = new HttpMessenger();
			messenger.RegisterCallback(this);   // Allows to get direct access to request and response

			// For printing the result to the log.
			jsonPrintOptions = new JsonOptions();
			jsonPrintOptions.CompactOutput = false;

			logBuilder = new StringBuilder();

			btnSendRequest.onClick.AddListener(SendRequest);
			btnClearLog.onClick.AddListener(ClearLog);
		}

		private void OnEnable()
		{
			ClearLog();
		}

		private void SendRequest()
		{
			LogMessage("Sending request to get the leaderboard:");
			GetLeaderboardRequest request = new GetLeaderboardRequest("test_01", 3, 0);
			request.ToString(logBuilder);
			LogMessage(string.Empty);

			messenger.SendRequest(request);
		}

		[HttpResponseCallback(typeof(GetLeaderboardResponse))]
		private void OnGetLeaderboardResponseReceived(HttpMessageHandle handle, GetLeaderboardRequest request, GetLeaderboardResponse response)
		{
			if (response != null)
			{
				logBuilder.AppendLine("Received get leaderboard response:");
				response.ToString(logBuilder);
				txtLog.text = logBuilder.ToString();
			}
			else if (handle.IsError)
			{
				LogMessage("An error occurred: " + handle.WebRequest.error);
			}
			else
			{
				LogMessage("The request did not complete successfully.");
			}
		}

		private void ClearLog()
		{
			logBuilder.Clear();
			txtLog.text = string.Empty;
		}

		private void LogMessage(string msg)
		{
			logBuilder.AppendLine(msg);
			txtLog.text = logBuilder.ToString();
		}
	}
}
