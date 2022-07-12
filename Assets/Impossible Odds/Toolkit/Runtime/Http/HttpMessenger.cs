namespace ImpossibleOdds.Http
{
	using System;
	using System.Text;
	using System.Collections;
	using UnityEngine.Networking;
	using ImpossibleOdds.Weblink;
	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Json;

	public class HttpMessenger : WeblinkMessenger<IHttpRequest, IHttpResponse, HttpMessageHandle, HttpResponseTypeAttribute, HttpResponseCallbackAttribute>
	{
		private IHttpMessageConfigurator messageConfigurator = null;

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
			this.messageConfigurator = configurator;
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

			UnityWebRequestAsyncOperation webOP = unityRequest.SendWebRequest();

			if (!webOP.isDone)
			{
				webOP.completed += (op) => OnRequestCompleted(messageHandle);
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
			UnityWebRequest unityWebRequest = null;

			if (request is IHttpGetRequest getRequest)
			{
				// Check whether the get request has a further specified type, e.g. texture or audio clip.
				if (getRequest is IHttpGetAudioClipRequest audioClipRequest)
				{
					unityWebRequest = UnityWebRequestMultimedia.GetAudioClip(url, audioClipRequest.AudioType);
				}
				else if (getRequest is IHttpGetTextureRequest textureRequest)
				{
					unityWebRequest = UnityWebRequestTexture.GetTexture(url);
				}
				else if (getRequest is IHttpGetAssetBundleRequest assetBundleRequest)
				{
					unityWebRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);
				}
				else
				{
					unityWebRequest = UnityWebRequest.Get(url);
				}
			}
			else if (request is IHttpPostRequest postRequest)
			{
				unityWebRequest = UnityWebRequest.Post(url, messageConfigurator.GenerateRequestPostBody(postRequest));
			}
			else if (request is IHttpPutStringRequest putStringRequest)
			{
				unityWebRequest = UnityWebRequest.Put(url, putStringRequest.PutData);
			}
			else if (request is IHttpPutBinaryRequest putBinaryRequest)
			{
				unityWebRequest = UnityWebRequest.Put(url, putBinaryRequest.PutData);
			}
			else
			{
				throw new HttpException("Request of type {0} could not be mapped to a supported type of {1}.", request.GetType().Name, typeof(UnityWebRequest).Name);
			}

			IDictionary headerData = messageConfigurator.GenerateRequestHeaders(request);
			if (headerData != null)
			{
				foreach (DictionaryEntry header in headerData)
				{
					unityWebRequest.SetRequestHeader(
						SerializationUtilities.PostProcessValue<string>(header.Key),
						SerializationUtilities.PostProcessValue<string>(header.Value));
				}
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

			UnityWebRequest webOP = handle.WebRequest;

			// Create the response and process the returned headers
			IHttpResponse response = InstantiateResponse(handle);
			messageConfigurator.ProcessResponseHeaders(response, webOP.GetResponseHeaders());

			// If the response expects structured data to be returned,
			if ((response is IHttpStructuredResponse structuredResponse) && !string.IsNullOrWhiteSpace(webOP.downloadHandler.text))
			{
				messageConfigurator.ProcessResponsePostBody(structuredResponse, webOP.downloadHandler.text);
			}
			else if (response is IHttpAudioClipResponse audioClipResponse)
			{
				audioClipResponse.AudioClip = DownloadHandlerAudioClip.GetContent(webOP);
			}
			else if (response is IHttpTextureResponse textureResponse)
			{
				textureResponse.Texture = DownloadHandlerTexture.GetContent(webOP);
			}
			else if (response is IHttpAssetBundleResponse assetBundleResponse)
			{
				assetBundleResponse.AssetBundle = DownloadHandlerAssetBundle.GetContent(webOP);
			}
			else if (response is IHttpCustomResponse customResponse)
			{
				try
				{
					customResponse.ProcessResponse(handle.WebRequest);
				}
				catch (Exception e)
				{
					Log.Exception(e);
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
			protected readonly ISerializationDefinition urlDefinition = null;
			protected readonly ISerializationDefinition headerDefinition = null;
			protected readonly ISerializationDefinition bodyDefinition = null;
			protected readonly StringBuilder processingCache = new StringBuilder();
			protected readonly JsonOptions jsonOptions = null;

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
				jsonOptions = new JsonOptions();
				jsonOptions.CompactOutput = true;
				jsonOptions.SerializationDefinition = bodyDefinition;
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
