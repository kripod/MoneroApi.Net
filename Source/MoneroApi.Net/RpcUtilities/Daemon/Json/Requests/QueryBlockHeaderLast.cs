namespace Jojatekok.MoneroAPI.RpcUtilities.Daemon.Json.Requests
{
    class QueryBlockHeaderLast : JsonRpcRequest
    {
        public QueryBlockHeaderLast() : base("getlastblockheader")
        {

        }
    }
}
