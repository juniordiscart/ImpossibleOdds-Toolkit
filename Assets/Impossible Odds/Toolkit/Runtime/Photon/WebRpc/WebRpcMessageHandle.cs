using ImpossibleOdds.Weblink;

namespace ImpossibleOdds.Photon.WebRpc
{
	public class WebRpcMessageHandle : IWeblinkMessageHandle<IWebRpcRequest, IWebRpcResponse>
	{
		/// <summary>
		/// The ID given by the WebRpcMessenger to uniquely identify a returned response by the server.
		/// </summary>
		public object RequestId { get; }

		/// <inheritdoc />
		public IWebRpcRequest Request { get; }

		/// <inheritdoc />
		public IWebRpcResponse Response { get; private set; }

		/// <summary>
		/// The debug message returned by the server, if any.
		/// </summary>
		public string DebugMessage { get; private set; }

		/// <inheritdoc />
		public bool IsDone => Response != null;

		/// <inheritdoc />
		public object Current => null;

		/// <inheritdoc />
		IWeblinkRequest IWeblinkMessageHandle.Request => Request;

		/// <inheritdoc />
		IWeblinkResponse IWeblinkMessageHandle.Response => Response;

		public WebRpcMessageHandle(IWebRpcRequest request, object requestId)
		{
			request.ThrowIfNull(nameof(request));
			requestId.ThrowIfNull(nameof(requestId));
			Request = request;
			RequestId = requestId;
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

			Response = response;
			DebugMessage = debugMessage;
		}
	}
}