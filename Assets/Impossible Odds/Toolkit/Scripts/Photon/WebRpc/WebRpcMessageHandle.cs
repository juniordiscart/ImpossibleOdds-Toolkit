namespace ImpossibleOdds.Photon.WebRpc
{
	using ImpossibleOdds.Weblink;

	public class WebRpcMessageHandle : IWeblinkMessageHandle<IWebRpcRequest, IWebRpcResponse>
	{
		private readonly string requestID = null;
		private readonly IWebRpcRequest request = null;
		private IWebRpcResponse response = null;
		private string debugMessage = null;

		/// <summary>
		/// The ID given by the WebRpcMessenger to uniquely identify a returned response by the server.
		/// </summary>
		public string RequestID
		{
			get { return requestID; }
		}

		/// <inheritdoc />
		public IWebRpcRequest Request
		{
			get { return request; }
		}

		/// <inheritdoc />
		public IWebRpcResponse Response
		{
			get { return response; }
		}

		/// <summary>
		/// The debug message returned by the server, if any.
		/// </summary>
		public string DebugMessage
		{
			get { return debugMessage; }
		}

		/// <inheritdoc />
		public bool IsDone
		{
			get { return response != null; }
		}

		/// <inheritdoc />
		public object Current
		{
			get { return null; }
		}

		/// <inheritdoc />
		IWeblinkRequest IWeblinkMessageHandle.Request
		{
			get { return Request; }
		}

		/// <inheritdoc />
		IWeblinkResponse IWeblinkMessageHandle.Response
		{
			get { return Response; }
		}

		public WebRpcMessageHandle(IWebRpcRequest request, string requestID)
		{
			request.ThrowIfNull(nameof(request));
			requestID.ThrowIfNullOrWhitespace(nameof(requestID));
			this.request = request;
			this.requestID = requestID;
		}

		/// <inheritdoc />
		public bool MoveNext()
		{
			return !IsDone;
		}

		/// <summary>
		/// Not implemented.
		/// </summary>
		public void Reset()
		{ }

		/// <summary>
		/// Set the returned response and its debug message.
		/// </summary>
		/// <param name="response">The processed response.</param>
		/// <param name="debugMessage">The debug message attached by the server.</param>
		public void SetResponse(IWebRpcResponse response, string debugMessage = null)
		{
			response.ThrowIfNull(nameof(response));

			this.response = response;
			this.debugMessage = debugMessage;
		}
	}
}
