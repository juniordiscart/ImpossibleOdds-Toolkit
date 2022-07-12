namespace ImpossibleOdds.Photon.WebRpc
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Concurrent;
	using ImpossibleOdds.Http;
	using ImpossibleOdds.Weblink;
	using ImpossibleOdds.Serialization;
	using global::Photon.Realtime;
	using ExitGames.Client.Photon;

	/// <summary>
	/// A messenger for WebRPC calls sent over the Photon multiplayer package.
	/// Implements the IWebRpcCallback interface so that it can be registered in the Photon callback targets.
	/// </summary>
	public class WebRpcMessenger : WeblinkMessenger<IWebRpcRequest, IWebRpcResponse, WebRpcMessageHandle, WebRpcResponseTypeAttribute, WebRpcResponseCallbackAttribute>,
		IWebRpcCallback,
		IDisposable
	{
		private readonly LoadBalancingClient photonClient = null;
		private IWebRpcMessageConfigurator messageConfigurator = null;
		private ConcurrentDictionary<object, WebRpcMessageHandle> pendingCalls = null;

		/// <summary>
		/// The configurator used by the messenger for handling the URL and body of the requests and incoming responses.
		/// Note: before switching the configurator, make sure all pending requests receive a response, or stop them.
		/// </summary>
		public IWebRpcMessageConfigurator MessageConfigurator
		{
			get => messageConfigurator;
			set
			{
				value.ThrowIfNull(nameof(value));
				if ((value != messageConfigurator) && (Count > 0))
				{
					throw new WebRpcException("Switching configurators while requests are still pending a response may cause requests to never complete. Stop all requests first before assigning a new configurator.");
				}

				messageConfigurator = value;
			}
		}

		public WebRpcMessenger(LoadBalancingClient photonClient)
		: this(photonClient, new DefaultMessageConfigurator())
		{ }

		public WebRpcMessenger(LoadBalancingClient photonClient, IWebRpcMessageConfigurator configurator)
		{
			photonClient.ThrowIfNull(nameof(photonClient));
			configurator.ThrowIfNull(nameof(configurator));

			this.photonClient = photonClient;
			this.messageConfigurator = configurator;
			this.pendingCalls = new ConcurrentDictionary<object, WebRpcMessageHandle>();

			photonClient.AddCallbackTarget(this);
		}

		/// <summary>
		/// Clears the messenger from the photon client.
		/// </summary>
		public void Dispose()
		{
			photonClient.RemoveCallbackTarget(this);
		}

		/// <inheritdoc />
		public override WebRpcMessageHandle SendRequest(IWebRpcRequest request)
		{
			request.ThrowIfNull(nameof(request));

			if (!photonClient.IsConnectedAndReady)
			{
				throw new WebRpcException("Photon isn't connected. Cannot send out any requests.");
			}

			// If the request is already pending, just return the existing handle.
			if (IsPending(request))
			{
				return GetMessageHandle(request);
			}

			return CustomOpWebRPC(request, messageConfigurator.GenerateRequestUri(request), messageConfigurator.GenerateRequestBody(request));
		}

		/// <inheritdoc />
		public override void StopAll()
		{
			base.StopAll();
			pendingCalls.Clear();
		}

		/// <inheritdoc />
		protected override void AddPendingRequest(WebRpcMessageHandle handle)
		{
			handle.ThrowIfNull(nameof(handle));

			base.AddPendingRequest(handle);
			pendingCalls[handle.RequestId] = handle;
		}

		/// <inheritdoc />
		protected override void RemovePendingRequest(IWebRpcRequest request)
		{
			request.ThrowIfNull(nameof(request));

			if (IsPending(request))
			{
				WebRpcMessageHandle handle = GetMessageHandle(request);
				pendingCalls.TryRemove(handle.RequestId, out _);
			}

			base.RemovePendingRequest(request);
		}

		/// <summary>
		/// Send a WebRPC operation through Photon. Since not all features are exposed through the
		/// Photon API, a custom send operation is created that applies missing features (e.g. encryption).
		/// </summary>
		/// <param name="request">The request to be sent.</param>
		/// <param name="uri">The request to be sent.</param>
		/// <param name="webRpcParams">The request to be sent.</param>
		/// <returns>A message handle when the request was delivered successfully to the Photon network.</returns>
		private WebRpcMessageHandle CustomOpWebRPC(IWebRpcRequest request, string uri, IDictionary webRpcParams)
		{
			request.ThrowIfNull(nameof(request));
			webRpcParams.ThrowIfNull(nameof(webRpcParams));
			uri.ThrowIfNullOrWhitespace(nameof(uri));

			// Generate the request identifier to identify the response.
			object requestId = string.Empty;
			do
			{
				requestId = messageConfigurator.GenerateAndApplyId(webRpcParams);
			} while (pendingCalls.ContainsKey(requestId));

			Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
			opParameters.Add(ParameterCode.UriPath, uri);
			opParameters.Add(ParameterCode.WebRpcParameters, webRpcParams);

			if (request.UseAuthCookie)
			{
				opParameters.Add(ParameterCode.EventForward, WebFlags.SendAuthCookieConst);
			}

			SendOptions options = SendOptions.SendReliable;
			options.Encrypt = request.UseEncryption;
			if (photonClient.LoadBalancingPeer.SendOperation(OperationCode.WebRpc, opParameters, options))
			{
				WebRpcMessageHandle messageHandle = new WebRpcMessageHandle(request, requestId);
				AddPendingRequest(messageHandle);
				return messageHandle;
			}
			else
			{
				throw new WebRpcException("Failed to send request of type {0} to {1}.", request.GetType().Name, request.UriPath);
			}
		}

		/// <inheritdoc />
		void IWebRpcCallback.OnWebRpcResponse(OperationResponse rawResponse)
		{
			switch (rawResponse.ReturnCode)
			{
				case -2:
					Log.Error("A WebRPC call failed. Is the Photon application configured correctly?\n{0}", rawResponse.DebugMessage);
					return;
				case 32745:
					Log.Error("A WebRPC call failed. Too many requests/second are being sent out.\n{0}", rawResponse.DebugMessage);
					return;
				case 32744:
					Log.Error("A WebRPC call failed. Something went wrong on the web service.\n{0}", rawResponse.DebugMessage);
					return;
			}

			WebRpcResponse webRpcResponse = new WebRpcResponse(rawResponse);

			// Check whether the response Id can be found and matched to a pending call.
			if (webRpcResponse.Parameters == null)
			{
				return;
			}

			object responseId = messageConfigurator.GetResponseId(webRpcResponse.Parameters);
			if ((responseId == null) || !pendingCalls.ContainsKey(responseId))
			{
				return;
			}

			WebRpcMessageHandle handle = pendingCalls[responseId];
			RemovePendingRequest(handle);

			IWebRpcResponse response = InstantiateResponse(handle);
			messageConfigurator.ProcessResponseBody(response, webRpcResponse.Parameters);
			handle.SetResponse(response, webRpcResponse.Message);

			if (handle.Response.IsSuccess)
			{
				HandleCompleted(handle);
			}
			else
			{
				HandleFailed(handle);
			}
		}

		/// <summary>
		/// Default configurator for preparing and configuring WebRPC requests and responses.
		/// </summary>
		public class DefaultMessageConfigurator : IWebRpcMessageConfigurator
		{
			public const ushort MinimumIdLength = 4;
			public const ushort DefaultIdLength = 8;
			public const string DefaultRequestIdKey = "RequestId";
			public const string DefaultResponseIdKey = "ResponseId";
			protected const string IdGenerationPool = "ABCDEFGHIJKLMNPQRSTUVWXYabcdefghjklmnopqrstuvwxyz0123456789_-";

			protected readonly ISerializationDefinition bodyDefinition = null;
			protected readonly ISerializationDefinition urlDefinition = null;
			protected readonly string requestIdKey = DefaultRequestIdKey;
			protected readonly string responseIdKey = DefaultResponseIdKey;
			private int generatedIdLength = DefaultIdLength;

			/// <summary>
			/// The length of generated identifiers.
			/// Must be at least 4 characters, or more.
			/// </summary>
			public virtual int GeneratedIdLength
			{
				get => generatedIdLength;
				set
				{
					if (value < MinimumIdLength)
					{
						throw new ArgumentOutOfRangeException(string.Format("The length of identifiers should at least be {0} long as to prevent collision.", MinimumIdLength));
					}

					generatedIdLength = value;
				}
			}

			/// <summary>
			/// Configurator with default options for serialization definitions used and the request and response identifier keys.
			/// </summary>
			public DefaultMessageConfigurator()
			: this(new WebRpcBodySerializationDefinition(), new WebRpcUrlSerializationDefinition(), DefaultRequestIdKey, DefaultResponseIdKey)
			{ }

			/// <summary>
			/// Configurator with customized serialization definitions, but with default request and response identifier keys.
			/// </summary>
			/// <param name="bodyDefinition">The serialization definition used for processing the body of the requests and responses.</param>
			/// <param name="urlDefinition">The serialization definition used for processing the URI parameters of the request.</param>
			public DefaultMessageConfigurator(ISerializationDefinition bodyDefinition, ISerializationDefinition urlDefinition)
			: this(bodyDefinition, urlDefinition, DefaultRequestIdKey, DefaultResponseIdKey)
			{ }

			/// <summary>
			/// Configurator with default serialization definitions, but with customized request and response identifier keys.
			/// </summary>
			/// <param name="requestIdKey">The identifier key used for setting the identifier in outgoing requests.</param>
			/// <param name="responseIdKey">The identifier key used for searching in incoming response data.</param>
			public DefaultMessageConfigurator(string requestIdKey, string responseIdKey)
			: this(new WebRpcBodySerializationDefinition(), new WebRpcUrlSerializationDefinition(), requestIdKey, responseIdKey)
			{ }

			/// <summary>
			/// Configurator with customized serialization definitions and request and response identifier keys.
			/// </summary>
			/// <param name="bodyDefinition">The serialization definition used for processing the body of the requests and responses.</param>
			/// <param name="urlDefinition">The serialization definition used for processing the URI parameters of the request.</param>
			/// <param name="requestIdKey">The identifier key used for setting the identifier in outgoing requests.</param>
			/// <param name="responseIdKey">The identifier key used for searching in incoming response data.</param>
			public DefaultMessageConfigurator(ISerializationDefinition bodyDefinition, ISerializationDefinition urlDefinition, string requestIdKey, string responseIdKey)
			{
				bodyDefinition.ThrowIfNull(nameof(bodyDefinition));
				urlDefinition.ThrowIfNull(nameof(urlDefinition));
				requestIdKey.ThrowIfNullOrWhitespace(nameof(requestIdKey));
				responseIdKey.ThrowIfNullOrWhitespace(nameof(responseIdKey));

				this.bodyDefinition = bodyDefinition;
				this.urlDefinition = urlDefinition;
				this.requestIdKey = requestIdKey;
				this.responseIdKey = responseIdKey;
			}

			/// <inheritdoc />
			public virtual string GenerateRequestUri(IWebRpcRequest request)
			{
				request.ThrowIfNull(nameof(request));

				string requestUri = request.UriPath;
				if (string.IsNullOrEmpty(requestUri))
				{
					throw new WebRpcException("The path of the request is null or empty.");
				}

				// Append parameters, if any.
				IDictionary urlParams = Serializer.Serialize<IDictionary>(request, urlDefinition);
				if (urlParams != null)
				{
					requestUri = UrlUtilities.BuildUrlQuery(requestUri, urlParams);
				}

				return requestUri;
			}

			/// <inheritdoc />
			public virtual IDictionary GenerateRequestBody(IWebRpcRequest request)
			{
				request.ThrowIfNull(nameof(request));

				// Generate the WebRPC parameters, and if none were generated, create one.
				IDictionary webRpcParams = Serializer.Serialize<IDictionary>(request, bodyDefinition);
				if (webRpcParams == null)
				{
					webRpcParams = new Dictionary<string, object>(1);   // At least one, because the request ID will be appended still.
				}

				return webRpcParams;
			}

			/// <inheritdoc />
			public virtual object GenerateAndApplyId(IDictionary requestBodyData)
			{
				requestBodyData.ThrowIfNull(nameof(requestBodyData));

				string requestId = GenerateRequestId();
				requestBodyData[requestIdKey] = requestId;
				return requestId;
			}

			/// <inheritdoc />
			public virtual object GetResponseId(IDictionary responseData)
			{
				responseData.ThrowIfNull(nameof(responseData));
				return responseData.Contains(responseIdKey) ? responseData[responseIdKey] : null;
			}

			/// <inheritdoc />
			public virtual void ProcessResponseBody(IWebRpcResponse response, IDictionary responseData)
			{
				response.ThrowIfNull(nameof(response));
				responseData.ThrowIfNull(nameof(responseData));
				Serializer.Deserialize(response, responseData, bodyDefinition);
			}

			/// <summary>
			/// Generates an identifier of the desired length and based on a pool of characters.
			/// Source; https://github.com/bolorundurowb/shortid
			/// </summary>
			/// <returns>An identifier composed of the characters in the character pool.</returns>
			private string GenerateRequestId()
			{
				char[] requestIdChars = new char[generatedIdLength];

				for (int i = 0; i < generatedIdLength; ++i)
				{
					requestIdChars[i] = IdGenerationPool[UnityEngine.Random.Range(0, IdGenerationPool.Length)];
				}

				return new string(requestIdChars);
			}
		}
	}
}
