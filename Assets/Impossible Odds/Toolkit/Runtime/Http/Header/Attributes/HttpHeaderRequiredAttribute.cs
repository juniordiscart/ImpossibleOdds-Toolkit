using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Http
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class HttpHeaderRequiredAttribute : Attribute, IRequiredParameter
    {
        public bool NullCheck { get; set; }
    }
}