using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcUtilities.Daemon.Json.Requests
{
    class QueryBlockHeaderByHash : JsonRpcRequest<QueryBlockHeaderByHashParameters>
    {
        public QueryBlockHeaderByHash(string hash) : base("getblockheaderbyhash", new QueryBlockHeaderByHashParameters(hash))
        {

        }
    }

    class QueryBlockHeaderByHashParameters
    {
        [JsonProperty("hash")]
        public string Hash { get; private set; }

        public QueryBlockHeaderByHashParameters(string hash)
        {
            Hash = hash;
        }
    }
}
