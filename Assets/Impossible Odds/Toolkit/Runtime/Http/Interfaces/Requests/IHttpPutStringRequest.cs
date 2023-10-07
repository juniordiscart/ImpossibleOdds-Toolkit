namespace ImpossibleOdds.Http
{
	/// <summary>
	/// Interface to denote that the request should be seen as a HTTP PUT request with a string payload.
	/// </summary>
	public interface IHttpPutStringRequest : IHttpRequest
	{
		/// <summary>
		/// The data to be sent to the the server in a string format.
		/// </summary>
		string PutData
		{
			get;
		}
	}
}