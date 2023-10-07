using System;
using System.Text;
using System.Collections;
using UnityEngine.Networking;
using ImpossibleOdds.Weblink;
using ImpossibleOdds.Serialization;
using ImpossibleOdds.Json;

namespace ImpossibleOdds.Http
{
	public class HttpMessenger : WeblinkMessenger<IHttpRequest, IHttpResponse, HttpMessageHandle, HttpResponseTypeAttribute, HttpResponseCallbackAttribute>
	{
		private IHttpMessageConfigurator messageConfigurator;

		/// <summary>
		/// The configurator being used for handling the URL, header and POST-body data.
		/// Note: before switching the configurator, make sure all pending requests receive a response, or stop them.
		/// </summary>
		public IHttpMessageConfigurator MessageConfigurator
		{
			get => messageConfigurator;
			set
			{
				value.ThrowIfNull(nameof(value));
				if ((value != messageConfigurator) && (Count > 0))
				{
					throw new HttpException("Switching configurators while requests are still pending a response may cause requests to never complete. Stop all requests first before assigning a new configurator.");
				}

				messageConfigurator = value;
			}
		}

		public HttpMessenger()
		: this(new DefaultMessageConfigurator())
		{ }

		public HttpMessenger(IHttpMessageConfigurator configurator)
		{
			configurator.ThrowIfNull(nameof(configurator));
			messageConfigurator = configurator;
		}

		/// <inheritdoc />
		public override HttpMessageHandle SendRequest(IHttpRequest request)
		{
			request.ThrowIfNull(nameof(request));

			// If it's already pending, then just return the existing handle.
			if (IsPending(request))
			{
				return GetMessageHandle(request);
			}

			UnityWebRequest unityRequest = GenerateWebRequest(request);
			HttpMessageHandle messageHandle = new HttpMessageHandle(request, unityRequest);
			AddPendingRequest(messageHandle);

			UnityWebRequestAsyncOperation webOp = unityRequest.SendWebRequest();

			if (!webOp.isDone)
			{
				webOp.completed += (op) => OnRequestCompleted(messageHandle);
			}
			else
			{
				// In case the operation completes immediately
				OnRequestCompleted(messageHandle);
			}

			return messageHandle;
		}

		private UnityWebRequest GenerateWebRequest(IHttpRequest request)
		{
			string url = messageConfigurator.GenerateRequestUrl(request);
			UnityWebRequest unityWebRequest;

			switch (request)
			{
				// Check whether the get request has a further specified type, e.g. texture or audio clip.
				case IHttpGetRequest getRequest when getRequest is IHttpGetAudioClipRequest audioClipRequest:
					unityWebRequest = UnityWebRequestMultimedia.GetAudioClip(url, audioClipRequest.AudioType);
					break;
				case IHttpGetRequest getRequest when getRequest is IHttpGetTextureRequest:
					unityWebRequest = UnityWebRequestTexture.GetTexture(url);
					break;
				case IHttpGetRequest getRequest when getRequest is IHttpGetAssetBundleRequest:
					unityWebRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);
					break;
				case IHttpGetRequest _:
					unityWebRequest = UnityWebRequest.Get(url);
					break;
				case IHttpPostRequest postRequest:
#if UNITY_2022_2_OR_NEWER
					unityWebRequest = UnityWebRequest.PostWwwForm(url, messageConfigurator.GenerateRequestPostBody(postRequest));
#else
					unityWebRequest = UnityWebRequest.Post(url, messageConfigurator.GenerateRequestPostBody(postRequest));
#endif
					break;
				case IHttpPutStringRequest putStringRequest:
					unityWebRequest = UnityWebRequest.Put(url, putStringRequest.PutData);
					break;
				case IHttpPutBinaryRequest putBinaryRequest:
					unityWebRequest = UnityWebRequest.Put(url, putBinaryRequest.PutData);
					break;
				default:
					throw new HttpException("Request of type {0} could not be mapped to a supported type of {1}.", request.GetType().Name, nameof(UnityWebRequest));
			}

			IDictionary headerData = messageConfigurator.GenerateRequestHeaders(request);
			if (headerData == null)
			{
				return unityWebRequest;
			}

			foreach (DictionaryEntry header in headerData)
			{
				unityWebRequest.SetRequestHeader(
					SerializationUtilities.PostProcessValue<string>(header.Key),
					SerializationUtilities.PostProcessValue<string>(header.Value));
			}

			return unityWebRequest;
		}

		private void OnRequestCompleted(HttpMessageHandle handle)
		{
			if (!IsPending(handle.Request))
			{
				return;
			}

			RemovePendingRequest(handle);

			if (handle.IsError)
			{
				HandleFailed(handle);
				return;
			}

			IHttpResponse response;
			using (UnityWebRequest webOp = handle.WebRequest)
			{
				response = InstantiateResponse(handle);
				messageConfigurator.ProcessResponseHeaders(response, webOp.GetResponseHeaders());

				switch (response)
				{
					// If the response expects structured data to be returned,
					case IHttpStructuredResponse structuredResponse when !string.IsNullOrWhiteSpace(webOp.downloadHandler.text):
						messageConfigurator.ProcessResponsePostBody(structuredResponse, webOp.downloadHandler.text);
						break;
					case IHttpAudioClipResponse audioClipResponse:
						audioClipResponse.AudioClip = DownloadHandlerAudioClip.GetContent(webOp);
						break;
					case IHttpTextureResponse textureResponse:
						textureResponse.Texture = DownloadHandlerTexture.GetContent(webOp);
						break;
					case IHttpAssetBundleResponse assetBundleResponse:
						assetBundleResponse.AssetBundle = DownloadHandlerAssetBundle.GetContent(webOp);
						break;
					case IHttpCustomResponse customResponse:
						try
						{
							customResponse.ProcessResponse(handle.WebRequest);
						}
						catch (Exception e)
						{
							Log.Exception(e);
						}

						break;
				}
			}

			handle.Response = response;
			HandleCompleted(handle);
		}

		/// <summary>
		/// Default configurator for preparing and configuring Http requests and responses.
		/// This configurator will use the JsonProcessor to transform the data for POST-type requests and responses.
		/// </summary>
		public class DefaultMessageConfigurator : IHttpMessageConfigurator
		{
			protected readonly ISerializationDefinition urlDefinition;
			protected readonly ISerializationDefinition headerDefinition;
			protected readonly ISerializationDefinition bodyDefinition;
			protected readonly StringBuilder processingCache = new StringBuilder();
			protected readonly JsonOptions jsonOptions;

			/// <summary>
			/// Configurator with default serialization definitions for the URL, headers and body of the requests and responses.
			/// </summary>
			public DefaultMessageConfigurator()
			: this(new HttpBodySerializationDefinition(), new HttpURLSerializationDefinition(), new HttpHeaderSerializationDefinition())
			{ }

			/// <summary>
			/// Configurator with customized serialization definitions for the URL, headers and body of the requests and responses.
			/// </summary>
			/// <param name="bodyDefinition"></param>
			/// <param name="urlDefinition"></param>
			/// <param name="headerDefinition"></param>
			public DefaultMessageConfigurator(ISerializationDefinition bodyDefinition, ISerializationDefinition urlDefinition, ISerializationDefinition headerDefinition)
			{
				bodyDefinition.ThrowIfNull(nameof(bodyDefinition));
				urlDefinition.ThrowIfNull(nameof(urlDefinition));
				headerDefinition.ThrowIfNull(nameof(headerDefinition));

				this.bodyDefinition = bodyDefinition;
				this.urlDefinition = urlDefinition;
				this.headerDefinition = headerDefinition;

				// Configure the JSON processing to use the Http Body serialization definition
				// instead of the default JSON serialization definition.
				jsonOptions = new JsonOptions
				{
					CompactOutput = true,
					SerializationDefinition = bodyDefinition
				};
			}

			/// <inheritdoc />
			public virtual string GenerateRequestUrl(IHttpRequest request)
			{
				request.ThrowIfNull(nameof(request));
				IDictionary urlParams = Serializer.Serialize<IDictionary>(request, urlDefinition);

				if ((urlParams == null) || (urlParams.Count == 0))
				{
					return request.URL;
				}

				return UrlUtilities.BuildUrlQuery(request.URL, urlParams);
			}

			/// <inheritdoc />
			public virtual IDictionary GenerateRequestHeaders(IHttpRequest request)
			{
				request.ThrowIfNull(nameof(request));
				return Serializer.Serialize<IDictionary>(request, headerDefinition);
			}

			/// <inheritdoc />
			public virtual string GenerateRequestPostBody(IHttpPostRequest request)
			{
				request.ThrowIfNull(nameof(request));

				JsonProcessor.Serialize(request, processingCache, jsonOptions);
				string result = processingCache.ToString();
				processingCache.Clear();

				return result;
			}

			/// <inheritdoc />
			public virtual void ProcessResponseHeaders(IHttpResponse response, IDictionary headers)
			{
				response.ThrowIfNull(nameof(response));

				if ((headers != null) && (headers.Count > 0))
				{
					Serializer.Deserialize(response, headers, headerDefinition);
				}
			}

			/// <inheritdoc />
			public virtual void ProcessResponsePostBody(IHttpStructuredResponse response, string postBodyData)
			{
				response.ThrowIfNull(nameof(response));
				postBodyData.ThrowIfNullOrWhitespace(nameof(postBodyData));
				JsonProcessor.Deserialize(response, postBodyData, jsonOptions);
			}
		}
	}
}