using System;

namespace ImpossibleOdds.Http
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class OnHttpURLDeserializingAttribute : Attribute
    { }
}