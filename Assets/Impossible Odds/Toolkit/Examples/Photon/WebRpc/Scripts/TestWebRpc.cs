#if PHOTON_UNITY_NETWORKING
#define PHOTON_AVAILABLE
#endif

namespace ImpossibleOdds.Examples.Photon.WebRpc
{
#if PHOTON_AVAILABLE
	using System.Text;
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;
	using ImpossibleOdds.Photon.WebRpc;

	public class TestWebRpc : MonoBehaviour
	{
		[SerializeField, HideInInspector]
		private DemoNetwork network = null;

		[SerializeField]
		private Button btnConnect = null;
		[SerializeField]
		private Button btnSendRequest = null;
		[SerializeField]
		private Button btnClearLog = null;
		[SerializeField]
		private TextMeshProUGUI txtLog = null;

		private StringBuilder logBuilder = null;

		private WebRpcMessenger messenger = null;

		private void Awake()
		{
			network.onConnected += OnConnected;
			network.onDisconnected += OnDisconnected;

			logBuilder = new StringBuilder();

			btnConnect.onClick.AddListener(Connect);
			btnSendRequest.onClick.AddListener(SendRequest);
			btnClearLog.onClick.AddListener(ClearLog);
		}

		private void OnEnable()
		{
			ClearLog();
		}

		private void Start()
		{
			// For this demo, the WebRpcMessenger is using a custom networking client
			// to connect to the demo server. It is not using the global PhotonServerSettings.
			// Instead use: messenger = new WebRpcMessenger(PhotonNetwork.NetworkingClient);
			messenger = new WebRpcMessenger(network.Client);
			messenger.RegisterCallback(this);

			btnSendRequest.interactable = network.IsConnected;
		}

		private void OnDestroy()
		{
			if (network != null)
			{
				network.PurgeDelegatesOf(this);
			}
		}

		private void Connect()
		{
			network.Connect();
			btnConnect.interactable = false;
			LogMessage("Connecting to Photon.");
		}

		private void OnConnected()
		{
			btnSendRequest.interactable = network.IsConnected;
			LogMessage("Connected to Photon. WebRPC is available for use.");
		}

		private void OnDisconnected()
		{
			btnConnect.interactable = true;
			btnSendRequest.interactable = network.IsConnected;
			LogMessage("Disconnected from Photon.");
		}

		private void SendRequest()
		{
			LogMessage("Sending request to update a leaderboard:");
			UpdateLeaderboardRequest request = new UpdateLeaderboardRequest(11, "best_race_times", UnityEngine.Random.Range(0, 9999));
			request.ToString(logBuilder);
			LogMessage(string.Empty);

			messenger.SendRequest(request);
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

		[WebRpcResponseCallback(typeof(UpdateLeaderboardResponse))]
		private void OnUpdateLeaderboardResponse(UpdateLeaderboardRequest request, UpdateLeaderboardResponse response, WebRpcMessageHandle handle)
		{
			LogMessage("Received update leaderboard respone:");
			response.ToString(logBuilder);

			if (!string.IsNullOrWhiteSpace(handle.DebugMessage))
			{
				LogMessage(string.Format("Debug message: {0}", handle.DebugMessage));
			}

			LogMessage(string.Empty);
		}
	}
#else

	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;

	// Stub, to keep references and save serialized data stored.
	public class TestWebRpc : MonoBehaviour
	{
		[SerializeField, HideInInspector]
		private DemoNetwork network = null;
		[SerializeField]
		private Button btnConnect = null;
		[SerializeField]
		private Button btnSendRequest = null;
		[SerializeField]
		private Button btnClearLog = null;
		[SerializeField]
		private TextMeshProUGUI txtLog = null;

		private void Start()
		{
			network.enabled = false;
			btnSendRequest.interactable = false;
			btnConnect.interactable = false;
			btnClearLog.interactable = false;
			txtLog.text = "Photon seems to be unavailable in the project. Make sure you have a valid PUN package installed in the project before running this demo.";
		}
	}
#endif
}
