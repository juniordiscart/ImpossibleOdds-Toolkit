using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Photon.WebRpc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
    public class WebRpcTypeIndexAttribute : Attribute, ISequenceTypeResolutionParameter
    {
        /// <inheritdoc />
        public Type Target { get; }

        /// <inheritdoc />
        public object Value { get; set; }

        /// <inheritdoc />
        public int IndexOverride { get; set; } = -1;

        public WebRpcTypeIndexAttribute(Type target)
        {
            target.ThrowIfNull(nameof(target));
            Target = target;
        }
    }
}