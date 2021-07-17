namespace ImpossibleOdds.Photon.WebRpc
{
	using System;
	using ImpossibleOdds.Weblink;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class WebRpcResponseTypeAttribute : WeblinkResponseAttribute
	{
		public WebRpcResponseTypeAttribute(Type responseType)
		: base(responseType)
		{ }
	}
}
