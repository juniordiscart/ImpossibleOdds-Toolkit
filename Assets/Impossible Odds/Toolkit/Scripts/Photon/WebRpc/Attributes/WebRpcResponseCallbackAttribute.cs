namespace ImpossibleOdds.Photon.WebRpc
{
	using System;
	using ImpossibleOdds.Weblink;

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class WebRpcResponseCallbackAttribute : WeblinkResponseCallbackAttribute
	{
		public WebRpcResponseCallbackAttribute(Type responseType)
		: base(responseType)
		{ }
	}
}
