using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcUtilities.Daemon.Http.Responses
{
    public class BlockHeightContainer : HttpRpcResponse
    {
        [JsonProperty("height")]
        public ulong BlockHeight { get; private set; }
    }
}
