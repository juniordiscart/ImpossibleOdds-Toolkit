using System.Collections;
using System.Collections.Generic;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Json
{
    public class JsonSequenceConfiguration : SequenceSerializationConfiguration<JsonArrayAttribute, JsonSequenceAttribute>
    {
        public JsonSequenceConfiguration()
            : base((i) => new List<object>(i))
        { }
    }
}