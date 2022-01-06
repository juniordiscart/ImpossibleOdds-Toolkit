namespace ImpossibleOdds.Http
{
	/// <summary>
	/// Interface to denote that the request should be seen as a PUT request with a binary payload.
	/// </summary>
	public interface IHttpPutBinaryRequest : IHttpRequest
	{
		byte[] PutData
		{
			get;
		}
	}
}
