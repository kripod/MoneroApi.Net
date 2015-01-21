using System;
using System.Net;

namespace Jojatekok.MoneroAPI.Settings
{
    public class RpcSettings
    {
        private string _urlHostDaemon;
        public string UrlHostDaemon {
            get { return _urlHostDaemon; }
            set {
                _urlHostDaemon = value;
                IsDaemonRemote = !new Uri(value).IsLoopback;
            }
        }

        public string UrlHostAccountManager { get; set; }

        public ushort UrlPortDaemon { get; set; }
        public ushort UrlPortAccountManager { get; set; }

        public bool IsDaemonRemote { get; private set; }

        public WebProxy Proxy { get; set; }

        public RpcSettings(string urlHostDaemon = Utilities.DefaultRpcUrlHostDaemon, string urlHostAccountManager = Utilities.DefaultRpcUrlHostAccountManager, ushort urlPortDaemon = Utilities.DefaultRpcUrlPortDaemon, ushort urlPortAccountManager = Utilities.DefaultRpcUrlPortAccountManager, bool isDaemonRemote = false, WebProxy proxy = null)
        {
            UrlHostDaemon = urlHostDaemon;
            UrlHostAccountManager = urlHostAccountManager;
            UrlPortDaemon = urlPortDaemon;
            UrlPortAccountManager = urlPortAccountManager;

            IsDaemonRemote = isDaemonRemote;

            Proxy = proxy;
        }
    }
}
