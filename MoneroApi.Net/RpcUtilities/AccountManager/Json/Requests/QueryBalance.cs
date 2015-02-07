namespace Jojatekok.MoneroAPI.RpcUtilities.AccountManager.Json.Requests
{
    public class QueryBalance : JsonRpcRequest
    {
        public QueryBalance() : base("getbalance")
        {

        }
    }
}
