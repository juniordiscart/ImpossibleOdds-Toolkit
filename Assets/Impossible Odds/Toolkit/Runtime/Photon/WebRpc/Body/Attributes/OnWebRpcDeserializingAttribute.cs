﻿using System;

namespace ImpossibleOdds.Photon.WebRpc
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class OnWebRpcDeserializingAttribute : Attribute
	{ }
}