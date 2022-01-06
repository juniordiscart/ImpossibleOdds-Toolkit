namespace ImpossibleOdds.Photon.WebRpc
{
	using ImpossibleOdds.Weblink;

	/// <summary>
	/// Interface that defines a WebRPC request.
	/// </summary>
	public interface IWebRpcRequest : IWeblinkRequest
	{
		/// <summary>
		/// The path on the server of the request.
		/// Note: this value is appended to the server-address value configured in your application dashboard on the Photon site.
		/// </summary>
		string UriPath
		{
			get;
		}

		/// <summary>
		/// Determines whether Photon should forward the AuthCookie with the request to the server.
		/// </summary>
		bool UseAuthCookie
		{
			get;
		}

		/// <summary>
		/// Determines whether Photon should encrypt the request when sending it out.
		/// </summary>
		bool UseEncryption
		{
			get;
		}
	}
}
