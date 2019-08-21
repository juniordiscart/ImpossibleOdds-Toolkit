namespace ImpossibleOdds.Photon.Webhooks
{
	using System;
	using ImpossibleOdds.Weblink;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class WebhookResponseAssociationAttribute : WeblinkResponseAttribute
	{
		public WebhookResponseAssociationAttribute(Type responseType)
		: base(responseType)
		{ }
	}
}
