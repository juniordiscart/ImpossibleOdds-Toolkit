namespace ImpossibleOdds.Photon.WebRpc
{
	using System;
	using ImpossibleOdds.Serialization;

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
	public sealed class WebRpcObjectAttribute : Attribute, ILookupTypeObject
	{ }
}
