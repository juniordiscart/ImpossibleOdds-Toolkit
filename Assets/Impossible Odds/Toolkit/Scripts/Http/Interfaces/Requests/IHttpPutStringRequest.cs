namespace ImpossibleOdds.Http
{
	/// <summary>
	/// Interface to denote that the request should be seen as a HTTP PUT request with a string payload.
	/// </summary>
	public interface IHttpPutStringRequest : IHttpRequest
	{
		string PutData
		{
			get;
		}
	}
}
