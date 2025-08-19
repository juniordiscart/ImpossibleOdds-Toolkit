using System.Collections.Generic;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Http
{
    public class HttpBodySequenceConfiguration : SequenceSerializationConfiguration<HttpBodyArrayAttribute, HttpBodySequenceAttribute>
    {
        public HttpBodySequenceConfiguration()
            : base((i) => new List<object>(i))
        { }
    }
}