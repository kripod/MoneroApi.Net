using Newtonsoft.Json;
using System.Collections.Generic;

namespace Jojatekok.MoneroAPI.RpcUtilities.AccountManager.Json.Requests
{
    class QueryPayments : JsonRpcRequest<QueryPaymentsParameters>
    {
        public QueryPayments(IList<string> paymentIds, ulong minimumBlockHeight) : base("get_bulk_payments", new QueryPaymentsParameters(paymentIds, minimumBlockHeight))
        {

        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    class QueryPaymentsParameters
    {
        [JsonProperty("payment_ids")]
        private IList<string> PaymentIds { get; set; }

        [JsonProperty("min_block_height")]
        private ulong MinimumBlockHeight { get; set; }

        public QueryPaymentsParameters(IList<string> paymentIds, ulong minimumBlockHeight)
        {
            PaymentIds = paymentIds;
            MinimumBlockHeight = minimumBlockHeight;
        }
    }
}
