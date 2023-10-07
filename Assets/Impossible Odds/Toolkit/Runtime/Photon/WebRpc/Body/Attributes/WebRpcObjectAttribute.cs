using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Photon.WebRpc
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public sealed class WebRpcObjectAttribute : Attribute, ILookupTypeObject
	{ }
}