using System.Collections;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Http
{
    public class HttpBodySequenceConfiguration : SequenceSerializationConfiguration<HttpBodyArrayAttribute, HttpBodySequenceAttribute>
    {
        public HttpBodySequenceConfiguration()
            : base((i) => new ArrayList(i))
        { }
    }
}