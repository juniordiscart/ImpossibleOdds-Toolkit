namespace ImpossibleOdds.Http
{
	using System.Text;
	using System.Collections;
	using System.Collections.Generic;

	using ImpossibleOdds.Weblink;
	using ImpossibleOdds.DataMapping;
	using ImpossibleOdds.Json;
	using ImpossibleOdds.Runnables;

	using UnityEngine;
	using UnityEngine.Networking;

	public class HttpMessenger : WeblinkMessenger<HttpAbstractRequest, HttpAbstractResponse, HttpResponseAssociationAttribute>
	{
		private static HttpURLMappingDefinition urlMappingDefinition = new HttpURLMappingDefinition();
		private static HttpHeaderMappingDefinition headerMappingDefinition = new HttpHeaderMappingDefinition();
		private static HttpBodyMappingDefinition bodyMappingDefinition = new HttpBodyMappingDefinition();

		protected override bool SendRequestData(HttpAbstractRequest request)
		{
			GlobalRunner.Default.StartCoroutine(ProcessRequest(request));
			return true;
		}

		protected override void ProcessResponseData(HttpAbstractRequest request, HttpAbstractResponse response, object responseData)
		{
			UnityWebRequest webRequest = responseData as UnityWebRequest;

			if (!webRequest.isDone)
			{
				throw new HttpException("The request has not completed yet.");
			}
			else if (webRequest.isNetworkError)
			{
				throw new HttpException(string.Format("Network error: {0}", webRequest.error));
			}
			else if (webRequest.isHttpError)
			{
				throw new HttpException(string.Format("HTTP error: {0}", webRequest.error));
			}

#if IMPOSSIBLE_ODDS_VERBOSE
			Debug.LogFormat("Received HTTP response {0} of type {1}.", request.ID, response.GetType().Name);
#endif

			// Map the headers.
			DataMapper.MapFromDataStructure(response, webRequest.GetResponseHeaders(), headerMappingDefinition);

			// If the response defines that a JSON response is expected.
			if (response is IHttpJsonResponse)
			{
				string jsonResponse = webRequest.downloadHandler.text;
				object jsonData = JsonProcessor.FromJson(jsonResponse);
				DataMapper.MapFromDataStructure(response, jsonData, bodyMappingDefinition);
			}

			// If the response defines a custom response handler, then we hand over the data.
			if (response is IHttpCustomResponseHandler)
			{
				(response as IHttpCustomResponseHandler).ProcessResponse(webRequest);
			}
		}

		private string GenerateURL(HttpAbstractRequest request)
		{
			Dictionary<string, string> urlParams = DataMapper.MapToDataStructure(request, urlMappingDefinition) as Dictionary<string, string>;

			if ((urlParams == null) || (urlParams.Count == 0))
			{
				return request.URIPath;
			}

			StringBuilder addressBuilder = new StringBuilder(request.URIPath);
			addressBuilder.Append('?');

			Dictionary<string, string>.Enumerator it = urlParams.GetEnumerator();
			int count = 0;

			while (it.MoveNext())
			{
				if (!string.IsNullOrEmpty(it.Current.Value))
				{
					if (count > 0)
					{
						addressBuilder.Append('&');
					}
					++count;

					addressBuilder
						.Append(UnityWebRequest.EscapeURL(it.Current.Key))
						.Append('=')
						.Append(UnityWebRequest.EscapeURL(it.Current.Value));
				}
			}

			return addressBuilder.ToString();
		}

		private IEnumerator ProcessRequest(HttpAbstractRequest request)
		{
			string url = GenerateURL(request);
			UnityWebRequest unityRequest = null;

			// Generate a request from one of Unity's templates
			switch (request.Method)
			{
				case HttpAbstractRequest.RequestMethod.GET:
					unityRequest = UnityWebRequest.Get(url);
					break;
				case HttpAbstractRequest.RequestMethod.POST:
					object postData = DataMapper.MapToDataStructure(request, bodyMappingDefinition);
					if (postData is Dictionary<string, string>)
					{
						unityRequest = UnityWebRequest.Post(url, postData as Dictionary<string, string>);
					}
					else
					{
						throw new HttpException(string.Format("The request of type {0} did not return valid POST data in the form of {1}.", request.GetType().Name, typeof(Dictionary<string, string>).Name));
					}
					break;
				case HttpAbstractRequest.RequestMethod.PUT:
					if (request is IHttpPutBinaryRequest)
					{
						unityRequest = UnityWebRequest.Put(url, (request as IHttpPutBinaryRequest).PutData);
					}
					else if (request is IHttpPutStringRequest)
					{
						unityRequest = UnityWebRequest.Put(url, (request as IHttpPutStringRequest).PutData);
					}
					else
					{
						throw new HttpException(string.Format("The request of type {0} does not implement either the {1} or {2} interfaces.", request.GetType().Name, typeof(IHttpPutBinaryRequest).Name, typeof(IHttpPutStringRequest).Name));
					}
					break;
				default:
					throw new HttpException(string.Format("Unhandled HTTP request method type: {0}.", request.Method.ToString()));
			}

			// Add the headers to the request
			Dictionary<string, string> headerData = DataMapper.MapToDataStructure(request, headerMappingDefinition) as Dictionary<string, string>;
			if (headerData != null)
			{
				foreach (KeyValuePair<string, string> header in headerData)
				{
					unityRequest.SetRequestHeader(header.Key, header.Value);
				}
			}
			else
			{
				throw new HttpException(string.Format("The request of type {0} did not return valid header data in the form of {1}.", request.GetType().Name, typeof(Dictionary<string, string>).Name));
			}

#if IMPOSSIBLE_ODDS_VERBOSE
			Debug.LogFormat("Sending HTTP request {0} of type {1}.", request.ID, request.GetType().Name);
#endif

			yield return unityRequest.SendWebRequest();
			ReceiveResponseData(unityRequest);
		}
	}
}
