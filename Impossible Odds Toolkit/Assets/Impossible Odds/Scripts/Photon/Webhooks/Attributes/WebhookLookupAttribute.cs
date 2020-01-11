#if IMPOSSIBLE_ODDS_PHOTON
namespace ImpossibleOdds.Photon.Webhooks
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class WebhookLookupAttribute : Attribute, ILookupDataStructure
	{ }
}

#endif
