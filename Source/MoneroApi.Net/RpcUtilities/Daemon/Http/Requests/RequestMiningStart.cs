using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcUtilities.Daemon.Http.Requests
{
    class RequestMiningStart : JsonRpcRequest<RequestMiningStartParameters>
    {
        public RequestMiningStart(string accountAddress, ulong threadsCount) : base(HttpRpcCommands.DaemonRequestMiningStart, new RequestMiningStartParameters(accountAddress, threadsCount))
        {

        }
    }

    class RequestMiningStartParameters
    {
        [JsonProperty("miner_address")]
        private string AccountAddress { get; set; }

        [JsonProperty("threads_count")]
        private ulong ThreadsCount { get; set; }

        public RequestMiningStartParameters(string accountAddress, ulong threadsCount)
        {
            AccountAddress = accountAddress;
            ThreadsCount = threadsCount;
        }
    }
}
