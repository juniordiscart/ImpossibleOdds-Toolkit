namespace ImpossibleOdds.Photon.WebRpc
{
	using System.Collections;

	/// <summary>
	/// Interface for preparing and configuring WebRPC requests and responses.
	/// </summary>
	public interface IWebRpcMessageConfigurator
	{
		/// <summary>
		/// Generate the URI of the request.
		/// </summary>
		/// <param name="request">The request for which to generate the URI.</param>
		/// <returns>The URI appended with any parameters as defined by the request, if any.</returns>
		string GenerateRequestUri(IWebRpcRequest request);

		/// <summary>
		/// Generate the body of the request.
		/// </summary>
		/// <param name="request">The request for which the data of the body should be generated.</param>
		/// <returns>A lookup-structure representation of the request data/</returns>
		IDictionary GenerateRequestBody(IWebRpcRequest request);

		/// <summary>
		/// Generate and apply an identifier to the request body data.
		/// </summary>
		/// <param name="requestData">The request data for which the identifier should be generated.</param>
		/// <returns></returns>
		object GenerateAndApplyId(IDictionary requestData);

		/// <summary>
		/// Check and return the response data for the presence of a response identifier.
		/// </summary>
		/// <param name="responseData">The response data to check.</param>
		/// <returns>The response identifier, if available. Null otherwise.</returns>
		object GetResponseId(IDictionary responseData);

		/// <summary>
		/// Apply the response dato onto the response object.
		/// </summary>
		/// <param name="response">The response onto which the data should be applied.</param>
		/// <param name="responseData">The response data.</param>
		void ProcessResponseBody(IWebRpcResponse response, IDictionary responseData);
	}
}
