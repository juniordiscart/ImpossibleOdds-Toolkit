#if IMPOSSIBLE_ODDS_PHOTON

namespace ImpossibleOdds.Photon.Webhooks
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class WebhookIndexAttribute : Attribute, IIndexParameter
	{
		public int Index
		{
			get { return index; }
		}

		private readonly int index;

		public WebhookIndexAttribute(int index)
		{
			if (index < 0)
			{
				throw new ArgumentException(string.Format("Index should be greater than 0. {0} given.", index));
			}

			this.index = index;
		}
	}
}

#endif
