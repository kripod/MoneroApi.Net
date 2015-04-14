using Newtonsoft.Json;
using System.Collections.Generic;

namespace Jojatekok.MoneroAPI.RpcUtilities.AccountManager.Json.Responses
{
    class TransactionIdListValueContainer : IValueContainer<IList<string>>
    {
        [JsonProperty("tx_hash_list")]
        public IList<string> Value { get; private set; }
    }
}
