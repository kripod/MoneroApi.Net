using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcUtilities.AccountManager.Json.Responses
{
    class AddressValueContainer : IValueContainer<string>
    {
        [JsonProperty("address")]
        public string Value { get; private set; }
    }
}
