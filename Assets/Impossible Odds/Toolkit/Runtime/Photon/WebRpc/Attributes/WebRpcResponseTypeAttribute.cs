namespace ImpossibleOdds.Photon.WebRpc
{
	using System;
	using ImpossibleOdds.Weblink;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class WebRpcResponseTypeAttribute : WeblinkResponseAttribute
	{
		public WebRpcResponseTypeAttribute(Type responseType)
		: base(responseType)
		{
			if (!typeof(IWebRpcResponse).IsAssignableFrom(responseType))
			{
				throw new WebRpcException("Type {0} does not implement interface {1}.", responseType.Name, typeof(IWebRpcResponse).Name);
			}
		}
	}
}
