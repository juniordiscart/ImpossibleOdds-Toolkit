namespace ImpossibleOdds.Http
{
	using System.Globalization;

	using ImpossibleOdds.Weblink;

	using UnityEngine.Networking;

	public abstract class HttpAbstractRequest : IWeblinkRequest
	{
		public enum RequestMethod
		{
			GET,
			POST,
			PUT
		}

		private static int requestIDCount = 0;

		[HttpHeaderField("Id")]
		private int id;

		public HttpAbstractRequest()
		{
			id = requestIDCount++;
		}

		public int ID
		{
			get { return id; }
		}

		public abstract string URIPath
		{
			get;
		}

		public abstract RequestMethod Method
		{
			get;
		}

		/// <summary>
		/// Checks the respone data to see if it is a response to this request.
		/// </summary>
		/// <param name="responseData">The response data received from the server.</param>
		/// <returns>True if this data is a response to the request.</returns>
		public virtual bool IsResponseData(object responseData)
		{
			if (!(responseData is UnityWebRequest))
			{
				return false;
			}

			UnityWebRequest webRequest = (responseData as UnityWebRequest);

			if (string.IsNullOrEmpty(webRequest.GetResponseHeader("Id")))
			{
				return false;
			}

			string idHeader = webRequest.GetResponseHeader("Id");
			int idValue = -1;
			if (!int.TryParse(idHeader, out idValue))
			{
				return false;
			}

			return id == idValue;
		}
	}
}