#if IMPOSSIBLE_ODDS_PHOTON

namespace ImpossibleOdds.Photon.Webhooks
{
	using System.Collections.Generic;

	using ImpossibleOdds.Serialization;
	using ImpossibleOdds.Weblink;

	using global::Photon.Realtime;

	public abstract class WebhookAbstractRequest : IWeblinkRequest
	{
		private static int requestIDCount = 0;

		[WebhookField("Id")]
		private int id;

		public WebhookAbstractRequest()
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

		public virtual bool UseAuthCookie
		{
			get { return false; }
		}

		public virtual bool UseEncryption
		{
			get { return false; }
		}

		public bool IsResponseData(object responseData)
		{
			if (!(responseData is WebRpcResponse))
			{
				return false;
			}

			Dictionary<string, object> parameters = (responseData as WebRpcResponse).Parameters;
			if ((parameters == null) || !parameters.ContainsKey("Id"))
			{
				return false;
			}

			return id.Equals(parameters["Id"]);
		}
	}
}

#endif
