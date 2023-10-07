using System.Collections;
using ImpossibleOdds.Serialization;

namespace ImpossibleOdds.Photon.WebRpc
{
    /// <summary>
    /// Default sequence serialization configuration for WebRPC body processing.
    /// </summary>
    public class WebRpcBodySequenceConfiguration : SequenceSerializationConfiguration<WebRpcArrayAttribute, WebRpcSequenceAttribute>
    {
        public WebRpcBodySequenceConfiguration()
            :base((i) => new ArrayList(i))
        { }
    }

}