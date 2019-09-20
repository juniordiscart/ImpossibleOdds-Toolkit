#if IMPOSSIBLE_ODDS_PHOTON

namespace ImpossibleOdds.Photon.Webhooks
{
	using System;
	using ImpossibleOdds.DataMapping;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class WebhookFieldAttribute : Attribute, ILookupParameter
	{
		public object Key
		{
			get { return key; }
		}

		private readonly object key;

		public WebhookFieldAttribute(object key)
		{
			key.ThrowIfNull(nameof(key));
			this.key = key;
		}
	}
}

#endif
