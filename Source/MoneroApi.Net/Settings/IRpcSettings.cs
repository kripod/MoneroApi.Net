using System.Net;

namespace Jojatekok.MoneroAPI.Settings
{
    public interface IRpcSettings
    {
        string UrlHostDaemon { get; set;}
        ushort UrlPortDaemon { get; set; }

        string UrlHostAccountManager { get; set; }
        ushort UrlPortAccountManager { get; set; }

        IWebProxy Proxy { get; set; }
    }
}
