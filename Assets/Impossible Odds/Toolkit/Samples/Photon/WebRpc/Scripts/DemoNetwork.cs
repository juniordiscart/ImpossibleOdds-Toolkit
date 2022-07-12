#if PHOTON_UNITY_NETWORKING
#define PHOTON_AVAILABLE
#endif

namespace ImpossibleOdds.Examples.Photon.WebRpc
{

#if PHOTON_UNITY_NETWORKING
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using ImpossibleOdds;
	using global::Photon.Realtime;

	/// <summary>
	/// Demo network class for connecting to the Impossible Odds demo application.
	/// This should remain separate from any project-defined app settings, as it might break the demo scenario/setup.
	/// </summary>
	public class DemoNetwork : MonoBehaviour, IConnectionCallbacks
	{
		public event Action onConnected;
		public event Action onDisconnected;

		[SerializeField, HideInInspector]
		private string demoAppId = string.Empty;

		private LoadBalancingClient client = null;

		public bool IsConnected
		{
			get => client.IsConnectedAndReady;
		}

		public LoadBalancingClient Client
		{
			get => client;
		}

		public void Connect()
		{
			AppSettings appSettings = new AppSettings()
			{
				AppIdRealtime = demoAppId
			};

			if (!client.ConnectUsingSettings(appSettings))
			{
				Log.Error("Failed to initialize the connection to Photon.");
			}
		}

		private void Awake()
		{
			client = new LoadBalancingClient();
			client.AddCallbackTarget(this);
		}

		private void OnDestroy()
		{
			client.RemoveCallbackTarget(this);
		}

		private void Update()
		{
			client.Service();
		}

		void IConnectionCallbacks.OnConnected()
		{ }

		void IConnectionCallbacks.OnConnectedToMaster()
		{
			onConnected.InvokeIfNotNull();
		}

		void IConnectionCallbacks.OnCustomAuthenticationFailed(string debugMessage)
		{ }

		void IConnectionCallbacks.OnCustomAuthenticationResponse(Dictionary<string, object> data)
		{ }

		void IConnectionCallbacks.OnDisconnected(DisconnectCause cause)
		{
			onDisconnected.InvokeIfNotNull();
		}

		void IConnectionCallbacks.OnRegionListReceived(RegionHandler regionHandler)
		{ }
	}
#else

	using UnityEngine;

	// Stub, to keep store references and save serialized data.
	public class DemoNetwork : MonoBehaviour
	{
		[SerializeField, HideInInspector]
		private string demoAppId = string.Empty;
	}
#endif
}
