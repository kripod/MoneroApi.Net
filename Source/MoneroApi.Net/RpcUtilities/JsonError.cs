using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcUtilities
{
    public class JsonError
    {
        [JsonProperty("code")]
        public int Code { get; private set; }

        [JsonProperty("message")]
        public string Message { get; private set; }
    }
}
