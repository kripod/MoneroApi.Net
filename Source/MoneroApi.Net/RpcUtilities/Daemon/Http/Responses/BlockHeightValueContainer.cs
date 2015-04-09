using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcUtilities.Daemon.Http.Responses
{
    class BlockHeightValueContainer : HttpRpcResponse, IValueContainer<ulong>
    {
        [JsonProperty("height")]
        public ulong Value { get; private set; }
    }
}
