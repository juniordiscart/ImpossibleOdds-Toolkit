namespace ImpossibleOdds.Examples.Http
{
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;

	using ImpossibleOdds.Http;
	using ImpossibleOdds.Json;
	using System.Text;

	public class TestHttp : MonoBehaviour
	{
		[SerializeField]
		private Button btnSendRequest = null;
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

			jsonPrintOptions = new JsonOptions();
			jsonPrintOptions.CompactOutput = false;

			logBuilder = new StringBuilder();

			btnSendRequest.onClick.AddListener(SendRequest);
		}

		private void OnEnable()
		{
			logBuilder.Clear();
			txtLog.text = string.Empty;
		}

		private void SendRequest()
		{
			logBuilder.Clear();
			GetLeaderboardRequest request = new GetLeaderboardRequest("test_01", 3, 0);
			messenger.SendRequest(request);
			LogMessage(string.Format("Sending request of type {0}.", request.GetType().Name));
		}

		[HttpResponseCallback(typeof(GetLeaderboardResponse))]
		private void OnGetLeaderboardResponseReceived(HttpMessageHandle handle, GetLeaderboardRequest request, GetLeaderboardResponse response)
		{
			if ((response != null) && response.IsSuccess)
			{
				logBuilder.AppendLine("Successfully retrieved leaderboard:");
				JsonProcessor.Serialize(response.Leaderboard, jsonPrintOptions, logBuilder);
				txtLog.text = logBuilder.ToString();
			}
			else if (handle.WebRequest.isNetworkError)
			{
				LogMessage("Network error: " + handle.WebRequest.error);
			}
			else if (handle.WebRequest.isHttpError)
			{
				LogMessage("HTTP error: " + handle.WebRequest.error);
			}
			else
			{
				LogMessage("The request did not complete successfully.");
			}
		}

		private void LogMessage(string msg)
		{
			logBuilder.AppendLine(msg);
			txtLog.text = logBuilder.ToString();
		}
	}
}
