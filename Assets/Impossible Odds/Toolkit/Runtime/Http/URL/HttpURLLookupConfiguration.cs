using System.Collections.Generic;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Http
{
    public class HttpURLLookupConfiguration : LookupSerializationConfiguration<HttpHeaderObjectAttribute, HttpHeaderFieldAttribute>
    {
        public HttpURLLookupConfiguration()
            : base((i) => new Dictionary<string, string>())
        { }
    }
}