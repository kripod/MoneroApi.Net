using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcUtilities.Daemon.Json.Requests
{
    class QueryBlockHeaderByHeight : JsonRpcRequest<QueryBlockHeaderByHeightParameters>
    {
        public QueryBlockHeaderByHeight(ulong height) : base("getblockheaderbyheight", new QueryBlockHeaderByHeightParameters(height))
        {

        }
    }

    class QueryBlockHeaderByHeightParameters
    {
        [JsonProperty("height")]
        public ulong Height { get; private set; }

        public QueryBlockHeaderByHeightParameters(ulong height)
        {
            Height = height;
        }
    }
}
