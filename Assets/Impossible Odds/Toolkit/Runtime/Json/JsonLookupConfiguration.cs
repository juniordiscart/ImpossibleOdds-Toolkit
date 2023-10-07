using System.Collections.Generic;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Json
{
    /// <summary>
    /// Default lookup serialization configuration for Json processing.
    /// </summary>
    public class JsonLookupConfiguration : LookupSerializationConfiguration<JsonObjectAttribute, JsonFieldAttribute>
    {
        public JsonLookupConfiguration()
            : base((i) => new Dictionary<string, object>())
        { }
    }
}