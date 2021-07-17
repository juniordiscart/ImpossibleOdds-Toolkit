namespace ImpossibleOdds.Photon.WebRpc
{
	using System;

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class OnWebRpcSerializingAttribute : Attribute
	{ }
}
