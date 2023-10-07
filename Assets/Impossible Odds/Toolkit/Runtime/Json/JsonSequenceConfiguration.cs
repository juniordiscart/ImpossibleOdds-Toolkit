using System.Collections;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Json
{
    public class JsonSequenceConfiguration : SequenceSerializationConfiguration<JsonArrayAttribute, JsonSequenceAttribute>
    {
        public JsonSequenceConfiguration()
            : base((i) => new ArrayList(i))
        { }
    }
}