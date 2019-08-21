#if IMPOSSIBLE_ODDS_PHOTON

namespace ImpossibleOdds.Photon.Webhooks
{
	using ImpossibleOdds.Weblink;
	using ImpossibleOdds.DataMapping;

	using System;
	using System.Collections.Generic;

	using global::Photon.Realtime;
	using global::Photon.Pun;

	using UnityEngine;
	using ExitGames.Client.Photon;

	public class WebhookMessenger : WeblinkMessenger<WebhookAbstractRequest, WebhookAbstractResponse, WebhookResponseAssociationAttribute>, IWebRpcCallback
	{
		private static readonly WebhookMappingDefinition mappingDefinition = new WebhookMappingDefinition();

		public WebhookMessenger()
		{
			PhotonNetwork.AddCallbackTarget(this);
		}

		public void OnWebRpcResponse(OperationResponse response)
		{
			if (response.ReturnCode != 0)
			{
#if IMPOSSIBLE_ODDS_VERBOSE
				Debug.LogErrorFormat("A WebRPC call did not complete successfully. Return code: {0}.", response.ReturnCode);
#endif
				return;
			}

			WebRpcResponse webRpcResponse = new WebRpcResponse(response);
			ReceiveResponseData(webRpcResponse);
		}

		protected override bool SendRequestData(WebhookAbstractRequest request)
		{
			object requestData = DataMapper.MapToDataStructure(request, mappingDefinition);
			return CustomOpWebRPC(request, requestData);
		}

		protected override void ProcessResponseData(WebhookAbstractRequest request, WebhookAbstractResponse response, object responseData)
		{
			WebRpcResponse photonResponse = responseData as WebRpcResponse;

#if IMPOSSIBLE_ODDS_VERBOSE
			if (string.IsNullOrEmpty(photonResponse.DebugMessage))
			{
				Debug.Log(photonResponse.DebugMessage);
			}
#endif

			Dictionary<string, object> parameters = photonResponse.Parameters as Dictionary<string, object>;
			DataMapper.MapFromDataStructure(response, parameters, mappingDefinition);
		}

		// Since ExitGames still doesn't allow sending of WebRPC calls with full options,
		// temporarily implement a custom version that DOES take into account the request preferences.
		private bool CustomOpWebRPC(WebhookAbstractRequest request, object parameters)
		{
			Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
			opParameters.Add(ParameterCode.UriPath, request.URIPath);
			opParameters.Add(ParameterCode.WebRpcParameters, parameters);
			if (request.UseAuthCookie)
			{
				opParameters.Add(ParameterCode.EventForward, WebFlags.SendAuthCookieConst);
			}

			SendOptions options = SendOptions.SendReliable;
			options.Encrypt = request.UseEncryption;
			return PhotonNetwork.NetworkingClient.LoadBalancingPeer.SendOperation(OperationCode.WebRpc, opParameters, options);
		}
	}
}

#endif
