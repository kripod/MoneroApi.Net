using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcUtilities.Daemon.Json.Responses
{
    class BlockHeaderValueContainer : IValueContainer<BlockHeader>
    {
        [JsonProperty("block_header")]
        public BlockHeader Value { get; private set; }
    }
}
