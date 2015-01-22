using System.Net;

namespace Jojatekok.MoneroAPI.Settings
{
    public interface IRpcSettings
    {
        string UrlHostDaemon { get; set;}
        string UrlHostAccountManager { get; set; }

        ushort UrlPortDaemon { get; set; }
        ushort UrlPortAccountManager { get; set; }

        bool IsDaemonRemote { get; }

        WebProxy Proxy { get; set; }
    }
}
