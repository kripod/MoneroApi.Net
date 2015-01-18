using System.Net;

namespace Jojatekok.MoneroAPI.Settings
{
    public class RpcSettings
    {
        public string UrlHost { get; set; }
        public ushort UrlPortDaemon { get; set; }
        public ushort UrlPortAccountManager { get; set; }

        public bool IsDaemonRemote { get; set; }
        public bool IsAccountManagerRemote { get; set; }

        public WebProxy Proxy { get; set; }

        public RpcSettings(string urlHost = Utilities.DefaultRpcUrlHost, ushort urlPortDaemon = Utilities.DefaultRpcUrlPortDaemon, ushort urlPortAccountManager = Utilities.DefaultRpcUrlPortAccountManager, bool isDaemonRemote = false, bool isAccountManagerRemote = false, WebProxy proxy = null)
        {
            UrlHost = urlHost;
            UrlPortDaemon = urlPortDaemon;
            UrlPortAccountManager = urlPortAccountManager;

            IsDaemonRemote = isDaemonRemote;
            IsAccountManagerRemote = isAccountManagerRemote;

            Proxy = proxy;
        }
    }
}
