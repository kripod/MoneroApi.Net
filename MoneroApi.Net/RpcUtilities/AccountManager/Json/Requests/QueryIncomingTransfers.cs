using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcUtilities.AccountManager.Json.Requests
{
    class QueryIncomingTransfers : JsonRpcRequest<QueryIncomingTransfersParameters>
    {
        public QueryIncomingTransfers(QueryIncomingTransfersParameters.TransfersType transfersType) : base("incoming_transfers", new QueryIncomingTransfersParameters(transfersType))
        {

        }

        public QueryIncomingTransfers() : this(QueryIncomingTransfersParameters.TransfersType.All)
        {

        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    class QueryIncomingTransfersParameters
    {
        public enum TransfersType
        {
            All,
            Available,
            Unavailable
        }

        [JsonProperty("transfer_type")]
        private string TransfersTypeString { get; set; }

        public QueryIncomingTransfersParameters(TransfersType transfersType)
        {
            switch (transfersType) {
                case TransfersType.All:
                    TransfersTypeString = "all";
                    break;

                case TransfersType.Available:
                    TransfersTypeString = "available";
                    break;

                case TransfersType.Unavailable:
                    TransfersTypeString = "unavailable";
                    break;
            }
        }
    }
}
