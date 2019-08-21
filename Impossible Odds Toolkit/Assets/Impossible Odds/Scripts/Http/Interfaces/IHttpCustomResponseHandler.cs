namespace ImpossibleOdds.Http
{
	using UnityEngine.Networking;

	public interface IHttpCustomResponseHandler
	{
		void ProcessResponse(UnityWebRequest request);
	}
}