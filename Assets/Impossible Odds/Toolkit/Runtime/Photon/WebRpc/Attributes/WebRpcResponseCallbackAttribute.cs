﻿using System;
using ImpossibleOdds.Weblink;

namespace ImpossibleOdds.Photon.WebRpc
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class WebRpcResponseCallbackAttribute : WeblinkResponseCallbackAttribute
	{
		public WebRpcResponseCallbackAttribute(Type responseType)
		: base(responseType)
		{
			if (!typeof(IWebRpcResponse).IsAssignableFrom(responseType))
			{
				throw new WebRpcException("The type {0} does not implement the {1} interface. This type cannot be used as a response callback type.", responseType.Name, nameof(IWebRpcResponse));
			}
		}
	}
}