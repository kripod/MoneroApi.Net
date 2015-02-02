namespace Jojatekok.MoneroAPI.RpcManagers
{
    public struct HttpRpcCommands
    {
        public const string DaemonQueryNetworkInformation = "getinfo";
        public const string DaemonQueryCurrentBlockHeight = "getheight";

        public const string DaemonQueryMiningStatus = "mining_status";

        public const string DaemonQueryTransactions = "gettransactions";
    }
}
