namespace ImpossibleOdds.Http
{
	using UnityEngine.Networking;

	/// <summary>
	/// Denotes the response expects to process the incoming response data in a custom way.
	/// </summary>
	public interface IHttpCustomResponse : IHttpResponse
	{
		void ProcessResponse(UnityWebRequest request);
	}
}
