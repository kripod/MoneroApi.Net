namespace Jojatekok.MoneroAPI.RpcUtilities
{
    public struct HttpRpcCommands
    {
        public const string DaemonQueryNetworkInformation = "getinfo";
        public const string DaemonQueryCurrentBlockHeight = "getheight";

        public const string DaemonQueryMiningStatus = "mining_status";

        public const string DaemonQueryTransactions = "gettransactions";

        public const string DaemonRequestMiningStart = "start_mining";
        public const string DaemonRequestMiningStop = "stop_mining";
    }
}
