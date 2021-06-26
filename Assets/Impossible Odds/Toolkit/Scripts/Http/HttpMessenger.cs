namespace ImpossibleOdds.Http
{
	using System;
	using System.Text;
	using System.Collections.Generic;
	using UnityEngine.Networking;
	using ImpossibleOdds.Weblink;
	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Json;

	public class HttpMessenger : WeblinkMessenger<IHttpRequest, IHttpResponse, HttpMessageHandle, HttpResponseTypeAttribute, HttpResponseCallbackAttribute>
	{
		private readonly static HttpURLSerializationDefinition urlDefinition = new HttpURLSerializationDefinition();
		private readonly static HttpHeaderSerializationDefinition headerDefinition = new HttpHeaderSerializationDefinition();
		private readonly static HttpBodySerializationDefinition bodyDefinition = new HttpBodySerializationDefinition();

		private StringBuilder stringBuilderCache = null;

		public HttpMessenger()
		{
			stringBuilderCache = new StringBuilder();
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
			// Log.Info("Sending request of type {0}.", request.GetType());

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
			string url = GenerateURL(request);
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
				stringBuilderCache.Clear();
				JsonProcessor.Serialize(Serializer.Serialize(request, bodyDefinition), stringBuilderCache);
				unityWebRequest = UnityWebRequest.Post(url, stringBuilderCache.ToString());
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

			Dictionary<string, string> headerData = Serializer.Serialize(request, headerDefinition) as Dictionary<string, string>;
			if (headerData != null)
			{
				foreach (KeyValuePair<string, string> header in headerData)
				{
					unityWebRequest.SetRequestHeader(header.Key, header.Value);
				}
			}

			return unityWebRequest;
		}

		private string GenerateURL(IHttpRequest request)
		{
			Dictionary<string, string> urlParams = Serializer.Serialize(request, urlDefinition) as Dictionary<string, string>;

			if ((urlParams == null) || (urlParams.Count == 0))
			{
				return request.URL;
			}

			return URLUtilities.BuildUrlQuery(request.URL, urlParams);
		}

		private void OnRequestCompleted(HttpMessageHandle handle)
		{
			if (IsPending(handle.Request))
			{
				RemovePendingRequest(handle);
			}

			if (handle.WebRequest.isNetworkError || handle.WebRequest.isHttpError)
			{
				HandleFailed(handle);
				return;
			}

			UnityWebRequest webOP = handle.WebRequest;

			// Create the response and process the returned headers
			IHttpResponse response = InstantiateResponse(handle);
			Serializer.Deserialize(response, webOP.GetResponseHeaders(), headerDefinition);

			// If the response expects JSON data to be returned,
			if ((response is IHttpJsonResponse) && !string.IsNullOrWhiteSpace(webOP.downloadHandler.text))
			{
				object jsonData = JsonProcessor.Deserialize(handle.WebRequest.downloadHandler.text);
				Serializer.Deserialize(response, jsonData, bodyDefinition);
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
	}
}
