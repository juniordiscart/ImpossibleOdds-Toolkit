using System;

namespace ImpossibleOdds.Http
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class OnHttpURLDeserializingAttribute : Attribute
    { }
}