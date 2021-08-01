namespace ImpossibleOdds.Http
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine.Networking;
	using ImpossibleOdds.Serialization;

	public static class UrlUtilities
	{
		private readonly static char[] QueryParamSplit = new char[] { '&' };

		/// <summary>
		/// Appends the parameters to the base URL. The base URL is allowed to have parameters defined already.
		/// Note: It is assumed the base URL already has been properly sanitized for use. Each parameter is sanitized.
		/// </summary>
		/// <param name="baseUrl">The base URL for which the parameters are appended.</param>
		/// <param name="parameters">The parameters to be appended.</param>
		/// <returns>The complete URL with parameters appended.</returns>
		public static string BuildUrlQuery(string baseUrl, IDictionary parameters)
		{
			baseUrl.ThrowIfNullOrWhitespace(nameof(baseUrl));
			parameters.ThrowIfNull(nameof(parameters));

			string result = baseUrl;
			foreach (DictionaryEntry it in parameters)
			{
				if ((it.Key == null) || (it.Value == null))
				{
					continue;
				}

				string key = SerializationUtilities.PostProcessValue<string>(it.Key);
				string value = SerializationUtilities.PostProcessValue<string>(it.Value);

				if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
				{
					result = AppendUrlParam(result, key, value);
				}
			}

			return result;
		}

		/// <summary>
		/// Appends a key-value pair to the URL.
		/// Note: the key and value will be escaped.
		/// </summary>
		/// <param name="baseUrl">The base URL on to which to append the key-value pair.</param>
		/// <param name="paramKey">The key to use for the URL parameter. The key will be properly URL-escaped.</param>
		/// <param name="paramValue">The value to use for the URL parameter. The value will be properly URL-escaped.</param>
		/// <returns>A concatenation of the key-value pair to the base URL in the form of baseURL?{key}={value}.</returns>
		public static string AppendUrlParam(string baseUrl, string paramKey, string paramValue)
		{
			paramKey.ThrowIfNullOrWhitespace(nameof(paramKey));
			paramValue.ThrowIfNullOrWhitespace(nameof(paramValue));
			return AppendUrlParam(baseUrl, string.Format("{0}={1}", UnityWebRequest.EscapeURL(paramKey), UnityWebRequest.EscapeURL(paramValue)));
		}

		/// <summary>
		/// Appends a key-value pair to the URL.
		/// Note: the key-value pair is assumed already URL-escaped and in the form of {key}={value}.
		/// </summary>
		/// <param name="baseUrl">The base URL on to which to append the key-value pair.</param>
		/// <param name="paramKeyValue">The key-value par to be appended the base URL.</param>
		/// <returns>A concatenation of the key-value pair to the base URL in the form of baseURL?{key}={value}.</returns>
		public static string AppendUrlParam(string baseUrl, string paramKeyValue)
		{
			baseUrl.ThrowIfNullOrWhitespace(nameof(baseUrl));
			paramKeyValue.ThrowIfNullOrWhitespace(nameof(paramKeyValue));

			if (baseUrl.Contains("?"))
			{
				return string.Format(baseUrl.EndsWith("&") ? "{0}{1}" : "{0}&{1}", baseUrl, paramKeyValue);
			}
			else
			{
				return string.Format("{0}?{1}", baseUrl, paramKeyValue);
			}
		}

		/// <summary>
		/// Extracts the parameters from the given URL.
		/// </summary>
		/// <param name="fullUrl">The full URL to extract the query parameters from.</param>
		/// <returns>A dictionary containing all query parameters. May be null if no query parameters were present in the URL.</returns>
		public static Dictionary<string, string> ExtractUrlQueryParameters(string fullUrl)
		{
			fullUrl.ThrowIfNullOrWhitespace(fullUrl);

			if (!fullUrl.Contains("?"))
			{
				return null;
			}

			string[] queryParams = fullUrl.Substring(fullUrl.IndexOf('?') + 1).Split(QueryParamSplit);
			Dictionary<string, string> result = new Dictionary<string, string>(queryParams.Length);

			foreach (string queryParam in queryParams)
			{
				if (queryParam.Contains("="))
				{
					int splitIndex = queryParam.IndexOf('=');
					string key = queryParam.Substring(0, splitIndex);
					string value = queryParam.Substring(splitIndex + 1, queryParam.Length - splitIndex - 1);

					result[key] = value;
				}
			}

			return result;
		}
	}
}
