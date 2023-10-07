using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Http
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
    public class HttpTypeIndexAttribute : Attribute, ISequenceTypeResolutionParameter
    {
        /// <inheritdoc />
        public Type Target { get; }

        /// <inheritdoc />
        public object Value { get; set; }

        /// <inheritdoc />
        public int IndexOverride { get; set; } = -1;

        public HttpTypeIndexAttribute(Type target)
        {
            target.ThrowIfNull(nameof(target));
            Target = target;
        }
    }
}