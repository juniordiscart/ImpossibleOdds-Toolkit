#if IMPOSSIBLE_ODDS_PHOTON
namespace ImpossibleOdds.Photon.Webhooks
{
	using System;
	using ImpossibleOdds.DataMapping;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class WebhookLookupMappingAttribute : Attribute, ILookupDataStructure
	{ }
}

#endif
