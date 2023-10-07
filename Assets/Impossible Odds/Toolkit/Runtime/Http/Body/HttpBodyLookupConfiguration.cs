using System.Collections.Generic;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Http
{
    public class HttpBodyLookupConfiguration : LookupSerializationConfiguration<HttpBodyObjectAttribute, HttpBodyFieldAttribute>
    {
        public HttpBodyLookupConfiguration()
            : base((i) => new Dictionary<string, string>())
        { }
    }
}