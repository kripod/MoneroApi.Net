using Jojatekok.MoneroAPI.Settings;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace Jojatekok.MoneroAPI.RpcUtilities
{
    sealed class RpcWebClient
    {
        public IRpcSettings RpcSettings { get; private set; }
        public ITimerSettings TimerSettings { get; private set; }

        private HttpClient HttpClient { get; set; }

        private static readonly JsonSerializer JsonSerializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore };
        private static readonly Encoding EncodingUtf8 = Encoding.UTF8;

        public RpcWebClient(IRpcSettings rpcSettings, ITimerSettings timerSettings)
        {
            RpcSettings = rpcSettings;
            TimerSettings = timerSettings;

            var httpClientHandler = new HttpClientHandler();
            if (httpClientHandler.SupportsAutomaticDecompression) {
                httpClientHandler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            }

            if (httpClientHandler.SupportsProxy) {
                var proxy = rpcSettings.Proxy;
                if (proxy != null) {
                    httpClientHandler.Proxy = proxy;
                    httpClientHandler.UseProxy = true;
                }
            }

            HttpClient = new HttpClient(httpClientHandler) {
                Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite)
            };

            HttpClient.DefaultRequestHeaders.ExpectContinue = false;
        }

        public T HttpPostData<T>(string host, ushort port, string command, JsonRpcRequest jsonRpcRequest = null)
        {
            string jsonString;
            if (jsonRpcRequest != null) {
                jsonString = PostString(host, port, command, JsonSerializer.SerializeObject(jsonRpcRequest));
            } else {
                jsonString = PostString(host, port, command);
            };

            return JsonSerializer.DeserializeObject<T>(jsonString);
        }

        public JsonRpcResponse<T> JsonPostData<T>(string host, ushort port, JsonRpcRequest jsonRpcRequest)
        {
            var jsonString = PostString(host, port, "json_rpc", JsonSerializer.SerializeObject(jsonRpcRequest));
            return JsonSerializer.DeserializeObject<JsonRpcResponse<T>>(jsonString);
        }

        private string PostString(string host, ushort port, string relativeUrl, string postData = null)
        {
            var requestUri = host + ":" + port + "/" + relativeUrl;
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);

            if (postData != null) {
                requestMessage.Content = new StringContent(postData, EncodingUtf8, "application/json");
            }

            var response = HttpClient.SendAsync(requestMessage).Result;
            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
