namespace ImpossibleOdds.Http
{
	using System.Collections;
	using UnityEngine.Networking;
	using ImpossibleOdds.Weblink;

	public class HttpMessageHandle : IWeblinkMessageHandle<IHttpRequest, IHttpResponse>
	{
		private readonly UnityWebRequest webRequest = null;
		private readonly IHttpRequest request = null;
		private IHttpResponse response = null;

		/// <summary>
		/// The request data.
		/// </summary>
		public IHttpRequest Request
		{
			get { return request; }
		}

		/// <summary>
		/// The actual web request that was sent out.
		/// </summary>
		public UnityWebRequest WebRequest
		{
			get { return webRequest; }
		}

		/// <summary>
		/// The response received.
		/// </summary>
		public IHttpResponse Response
		{
			get { return response; }
			set { response = value; }
		}

		/// <summary>
		/// True when a response has been received.
		/// </summary>
		public bool IsDone
		{
			get { return ((webRequest != null) && (webRequest.isNetworkError || webRequest.isHttpError)) || (response != null); }
		}

		/// <summary>
		/// True if an error occurred while sending the request.
		/// </summary>
		public bool IsError
		{
			get { return (webRequest != null) && (webRequest.isNetworkError || webRequest.isHttpError); }
		}

		/// <summary>
		/// Not implemented.
		/// </summary>
		object IEnumerator.Current
		{
			get { return null; }
		}

		IWeblinkRequest IWeblinkMessageHandle.Request
		{
			get { return Request; }
		}

		IWeblinkResponse IWeblinkMessageHandle.Response
		{
			get { return Response; }
		}

		public HttpMessageHandle(IHttpRequest request, UnityWebRequest webRequest)
		{
			request.ThrowIfNull(nameof(request));
			webRequest.ThrowIfNull(nameof(webRequest));

			this.request = request;
			this.webRequest = webRequest;
		}

		bool IEnumerator.MoveNext()
		{
			return !IsDone;
		}

		/// <summary>
		/// Not implemented.
		/// </summary>
		void IEnumerator.Reset()
		{ }
	}
}
