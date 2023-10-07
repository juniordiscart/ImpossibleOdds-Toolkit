using System;

namespace ImpossibleOdds.Http
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class OnHttpHeaderSerializedAttribute : Attribute
    { }
}