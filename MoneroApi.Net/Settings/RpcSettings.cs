using System.Net;

namespace Jojatekok.MoneroAPI.Settings
{
    public class RpcSettings : IRpcSettings
    {
        public string UrlHostDaemon { get; set; }
        public ushort UrlPortDaemon { get; set; }

        public string UrlHostAccountManager { get; set; }
        public ushort UrlPortAccountManager { get; set; }

        public IWebProxy Proxy { get; set; }

        public RpcSettings(string urlHostDaemon = Utilities.DefaultRpcUrlHostDaemon, ushort urlPortDaemon = Utilities.DefaultRpcUrlPortDaemon, string urlHostAccountManager = Utilities.DefaultRpcUrlHostAccountManager, ushort urlPortAccountManager = Utilities.DefaultRpcUrlPortAccountManager, IWebProxy proxy = null)
        {
            UrlHostDaemon = urlHostDaemon;
            UrlPortDaemon = urlPortDaemon;
            UrlHostAccountManager = urlHostAccountManager;
            UrlPortAccountManager = urlPortAccountManager;

            Proxy = proxy;
        }
    }
}
