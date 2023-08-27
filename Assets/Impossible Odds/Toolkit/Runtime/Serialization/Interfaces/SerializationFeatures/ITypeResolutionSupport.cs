using System;

namespace ImpossibleOdds.Serialization
{

    public interface ITypeResolutionFeature : ISerializationFeature
    {
        Type TypeResolutionAttribute
        {
            get;
        }
    }
}