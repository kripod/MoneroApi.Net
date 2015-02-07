namespace Jojatekok.MoneroAPI.RpcUtilities.Daemon.Json.Requests
{
    public class QueryBlockHeaderLast : JsonRpcRequest
    {
        internal QueryBlockHeaderLast() : base("getlastblockheader")
        {

        }
    }
}
