namespace ImpossibleOdds.Http
{
	/// <summary>
	/// Interface to denote that the request should be seen as a PUT request with a binary payload.
	/// </summary>
	public interface IHttpPutBinaryRequest : IHttpRequest
	{
		/// <summary>
		/// The binary data that should be sent to the server.
		/// </summary>
		byte[] PutData
		{
			get;
		}
	}
}