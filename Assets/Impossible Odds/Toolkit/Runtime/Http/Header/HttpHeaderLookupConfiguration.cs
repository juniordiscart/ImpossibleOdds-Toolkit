using System.Collections.Generic;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Http
{
    public class HttpHeaderLookupConfiguration : LookupSerializationConfiguration<HttpHeaderObjectAttribute, HttpHeaderFieldAttribute>
    {
        public HttpHeaderLookupConfiguration()
            : base((i) => new Dictionary<string, string>())
        { }
    }
}