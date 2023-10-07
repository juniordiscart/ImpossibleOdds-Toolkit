using System.Collections.Generic;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Photon.WebRpc
{
    /// <summary>
    /// Default lookup serialization configuration for WebRPC body processing.
    /// </summary>
    public class WebRpcBodyLookupConfiguration : LookupSerializationConfiguration<WebRpcObjectAttribute, WebRpcFieldAttribute>
    {
        public WebRpcBodyLookupConfiguration()
            : base((i) => new Dictionary<string, object>())
        { }
    }

}