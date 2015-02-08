using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcUtilities.AccountManager.Json.Requests
{
    class QueryKey : JsonRpcRequest<QueryKeyParameters>
    {
        public QueryKey(AccountKeyType accountKeyType) : base("query_key", new QueryKeyParameters(accountKeyType))
        {

        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    class QueryKeyParameters
    {
        [JsonProperty("key_type")]
        private string KeyTypeString { get; set; }

        public QueryKeyParameters(AccountKeyType accountKeyType)
        {
            switch (accountKeyType) {
                case AccountKeyType.ViewKey:
                    KeyTypeString = "view_key";
                    break;

                case AccountKeyType.SpendKey:
                    KeyTypeString = "spend_key";
                    break;

                case AccountKeyType.Mnemonic:
                    KeyTypeString = "mnemonic";
                    break;
            }
        }
    }
}
