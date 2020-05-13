#if IMPOSSIBLE_ODDS_PHOTON

namespace ImpossibleOdds.Photon.Webhooks
{
	using ImpossibleOdds.Weblink;

	public abstract class WebhookAbstractResponse : IWeblinkResponse
	{
		[WebhookField("Id")]
		private int id = 0;

		public int ID
		{
			get { return id; }
		}
	}
}

#endif
