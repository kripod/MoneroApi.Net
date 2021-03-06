﻿using Jojatekok.MoneroAPI.RpcUtilities;
using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI
{
    public class MiningStatus : HttpRpcResponse
    {
        [JsonProperty("active")]
        public bool IsActive { get; private set; }

        [JsonProperty("address")]
        public string Address { get; private set; }

        [JsonProperty("threads_count")]
        public uint ThreadsCount { get; private set; }

        [JsonProperty("speed")]
        public ulong Speed { get; private set; }
    }
}
