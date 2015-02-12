using Newtonsoft.Json;
using System.Collections.Generic;

namespace Jojatekok.MoneroAPI.RpcUtilities.AccountManager.Json.Responses
{
    class TransactionOutputListValueContainer : IValueContainer<IList<TransactionOutput>>
    {
        [JsonProperty("transfers")]
        public IList<TransactionOutput> Value { get; private set; }
    }
}
