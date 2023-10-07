using System.Collections.Generic;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Photon.WebRpc
{
    public class WebRpcURLLookupConfiguration : LookupSerializationConfiguration<WebRpcURLObjectAttribute, WebRpcUrlFieldAttribute>
    {
        public WebRpcURLLookupConfiguration()
            : base((i) => new Dictionary<string, string>(i))
        { }
    }
}