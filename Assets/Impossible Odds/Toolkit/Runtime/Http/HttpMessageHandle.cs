using System;
using System.Collections;
using UnityEngine.Networking;
using ImpossibleOdds.Weblink;

namespace ImpossibleOdds.Http
{
	public class HttpMessageHandle : IWeblinkMessageHandle<IHttpRequest, IHttpResponse>, IDisposable
	{
		/// <summary>
		/// The request data.
		/// </summary>
		public IHttpRequest Request { get; }

		/// <summary>
		/// The actual web request that was sent out.
		/// </summary>
		public UnityWebRequest WebRequest { get; }

		/// <summary>
		/// The response received.
		/// </summary>
		public IHttpResponse Response { get; set; }

		/// <summary>
		/// True when a response has been received.
		/// </summary>
		public bool IsDone
		{
#if UNITY_2020_2_OR_NEWER
			get => (Response != null) || IsError;
#else
			get => ((WebRequest != null) && (WebRequest.isNetworkError || WebRequest.isHttpError)) || (Response != null);
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
				switch (WebRequest.result)
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

			get => (WebRequest != null) && (WebRequest.isNetworkError || WebRequest.isHttpError);
#endif
		}

		/// <summary>
		/// Not implemented.
		/// </summary>
		object IEnumerator.Current => null;

		IWeblinkRequest IWeblinkMessageHandle.Request => Request;

		IWeblinkResponse IWeblinkMessageHandle.Response => Response;

		public HttpMessageHandle(IHttpRequest request, UnityWebRequest webRequest)
		{
			request.ThrowIfNull(nameof(request));
			webRequest.ThrowIfNull(nameof(webRequest));

			Request = request;
			WebRequest = webRequest;
		}

		public void Dispose()
		{
			WebRequest.DisposeIfNotNull();
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