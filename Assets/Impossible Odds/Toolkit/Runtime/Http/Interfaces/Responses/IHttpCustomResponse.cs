using UnityEngine.Networking;

namespace ImpossibleOdds.Http
{
	/// <summary>
	/// Denotes that the response expects to be processed in a custom way.
	/// </summary>
	public interface IHttpCustomResponse : IHttpResponse
	{
		/// <summary>
		/// Invoked when a response has been received.
		/// </summary>
		/// <param name="request">The associated request of the response.</param>
		void ProcessResponse(UnityWebRequest request);
	}
}