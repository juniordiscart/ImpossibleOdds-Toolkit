using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Http
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class HttpURLRequiredAttribute : Attribute, IRequiredParameter
    {
        public bool NullCheck { get; set; }
    }
}