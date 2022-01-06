namespace ImpossibleOdds.Photon.WebRpc
{
	using System;
	using ImpossibleOdds.Weblink;

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class WebRpcResponseCallbackAttribute : WeblinkResponseCallbackAttribute
	{
		public WebRpcResponseCallbackAttribute(Type responseType)
		: base(responseType)
		{
			if (!typeof(IWebRpcResponse).IsAssignableFrom(responseType))
			{
				throw new WebRpcException("The type {0} does not implement the {1} interface. This type cannot be used as a response callback type.", responseType.Name, typeof(IWebRpcResponse).Name);
			}
		}
	}
}
