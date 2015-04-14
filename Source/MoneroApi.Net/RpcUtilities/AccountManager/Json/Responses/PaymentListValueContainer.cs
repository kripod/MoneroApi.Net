using Newtonsoft.Json;
using System.Collections.Generic;

namespace Jojatekok.MoneroAPI.RpcUtilities.AccountManager.Json.Responses
{
    class PaymentListValueContainer : IValueContainer<IList<Payment>>
    {
        [JsonProperty("payments")]
        public IList<Payment> Value { get; private set; }
    }
}
