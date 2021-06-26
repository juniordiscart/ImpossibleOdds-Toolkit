namespace ImpossibleOdds.Http
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine.Networking;

	public static class URLUtilities
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

			List<string> paramsList = new List<string>(parameters.Count);
			foreach (IDictionaryEnumerator it in parameters)
			{
				if ((it.Key == null) || (it.Value == null))
				{
					continue;
				}

				string key = (it.Key is string) ? it.Key as string : it.Key.ToString();
				string value = (it.Value is string) ? it.Value as string : it.Value.ToString();

				if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
				{
					paramsList.Add(string.Format("{0}={1}", UnityWebRequest.EscapeURL(key), UnityWebRequest.EscapeURL(value)));
				}
			}

			string queryParams = string.Join("&", paramsList);

			if (baseUrl.Contains("?"))
			{
				return string.Format(baseUrl.EndsWith("&") ? "{0}{1}" : "{0}&{1}", baseUrl, queryParams);
			}
			else
			{
				return string.Format("{0}?{1}", baseUrl, queryParams);
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
