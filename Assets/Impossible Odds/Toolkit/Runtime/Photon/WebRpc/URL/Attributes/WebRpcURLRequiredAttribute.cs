using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Photon.WebRpc
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class WebRpcURLRequiredAttribute : Attribute, IRequiredParameter
    {
        public bool NullCheck { get; set; }
    }
}