using System;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Http
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class HttpURLRequiredAttribute : Attribute, IRequiredParameter
    {
        private bool performNullCheck = false;

        public bool NullCheck
        {
            get => performNullCheck;
            set => performNullCheck = value;
        }
    }
}