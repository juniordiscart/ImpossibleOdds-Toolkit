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
			get => request;
		}

		/// <summary>
		/// The actual web request that was sent out.
		/// </summary>
		public UnityWebRequest WebRequest
		{
			get => webRequest;
		}

		/// <summary>
		/// The response received.
		/// </summary>
		public IHttpResponse Response
		{
			get => response;
			set => response = value;
		}

		/// <summary>
		/// True when a response has been received.
		/// </summary>
		public bool IsDone
		{
#if UNITY_2020_2_OR_NEWER
			get => (response != null) || IsError;
#else
			get => ((webRequest != null) && (webRequest.isNetworkError || webRequest.isHttpError)) || (response != null);
#endif
		}

		/// <summary>
		/// True if an error occurred while sending the request.
		/// </summary>
		public bool IsError
		{
#if UNITY_2020_2_OR_NEWER
			get
			{
				switch (webRequest.result)
				{
					case UnityWebRequest.Result.ConnectionError:
					case UnityWebRequest.Result.ProtocolError:
					case UnityWebRequest.Result.DataProcessingError:
						return true;
					default:
						return false;
				}
			}
#else

			get => (webRequest != null) && (webRequest.isNetworkError || webRequest.isHttpError);
#endif
		}

		/// <summary>
		/// Not implemented.
		/// </summary>
		object IEnumerator.Current
		{
			get => null;
		}

		IWeblinkRequest IWeblinkMessageHandle.Request
		{
			get => Request;
		}

		IWeblinkResponse IWeblinkMessageHandle.Response
		{
			get => Response;
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
