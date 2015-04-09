using Newtonsoft.Json;

namespace Jojatekok.MoneroAPI.RpcUtilities
{
    public class HttpRpcResponse
    {
        [JsonProperty("status")]
        private string StatusInternal {
            set {
                switch (value.ToLower()) {
                    case "ok":
                        Status = RpcResponseStatus.Ok;
                        break;

                    case "busy":
                        Status = RpcResponseStatus.Busy;
                        break;

                    default:
                        Status = RpcResponseStatus.Error;
                        break;
                }
            }
        }
        public RpcResponseStatus Status { get; private set; }
    }
}
