namespace ImpossibleOdds.Http
{
	using System.Collections;

	/// <summary>
	/// Interface for preparing and configuring Http requests and responses.
	/// </summary>
	public interface IHttpMessageConfigurator
	{
		/// <summary>
		/// Generate the POST-body of the request.
		/// </summary>
		/// <param name="request">The request for which the data of the body should be generated.</param>
		/// <returns>A string representation of the request's body.</returns>
		string GenerateRequestPostBody(IHttpPostRequest request);

		/// <summary>
		/// Generate the URL of the request.
		/// </summary>
		/// <param name="request">The request for which to generate the URL.</param>
		/// <returns>The full URL of the request.</returns>
		string GenerateRequestUrl(IHttpRequest request);

		/// <summary>
		/// Generate additional headers of the request.
		/// These headers will be appended to Unity's default set of headers generated for each request.
		/// </summary>
		/// <param name="request">The request for which to generate the headers.</param>
		/// <returns>A set of additional headers for the request.</returns>
		IDictionary GenerateRequestHeaders(IHttpRequest request);

		/// <summary>
		/// Apply the set of response headers on the response object.
		/// </summary>
		/// <param name="response">The response onto which the header values should be applied.</param>
		/// <param name="headers">The headers found in the response data.</param>
		void ProcessResponseHeaders(IHttpResponse response, IDictionary headers);

		/// <summary>
		/// Apply the POST-body data of the response to the response object.
		/// </summary>
		/// <param name="response">The response onto which the POST-body data should be applied.</param>
		/// <param name="postBodyData">The POST-body data found in the response.</param>
		void ProcessResponsePostBody(IHttpStructuredResponse response, string postBodyData);
	}
}
