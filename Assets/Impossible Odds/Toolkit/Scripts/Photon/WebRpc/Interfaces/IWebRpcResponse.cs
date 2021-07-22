namespace ImpossibleOdds.Photon.WebRpc
{
	using ImpossibleOdds.Weblink;

	/// <summary>
	/// Dummy interface for defining a WebRPC response.
	/// </summary>
	public interface IWebRpcResponse : IWeblinkResponse
	{
		/// <summary>
		/// State whether the request was completed successfully on the server.
		/// </summary>
		bool IsSuccess
		{
			get;
		}
	}
}
