using System;
using ImpossibleOdds.Weblink;

namespace ImpossibleOdds.Photon.WebRpc
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class WebRpcResponseTypeAttribute : WeblinkResponseAttribute
	{
		public WebRpcResponseTypeAttribute(Type responseType)
		: base(responseType)
		{
			if (!typeof(IWebRpcResponse).IsAssignableFrom(responseType))
			{
				throw new WebRpcException("Type {0} does not implement interface {1}.", responseType.Name, nameof(IWebRpcResponse));
			}
		}
	}
}