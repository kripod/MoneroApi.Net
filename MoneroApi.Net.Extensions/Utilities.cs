using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace Jojatekok.MoneroAPI.Extensions
{
    public static class Utilities
    {
        private static readonly string DefaultRelativePathDirectorySoftware = Path.Combine("Resources", "Software");
        private const string DefaultRelativePathDirectoryAccountData = "AccountData";

        public static readonly string DefaultPathSoftwareDaemon = Path.Combine(DefaultRelativePathDirectorySoftware, "bitmonerod.exe");
        public static readonly string DefaultPathDirectoryDaemonData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "bitmonero");

        public static readonly string DefaultPathSoftwareAccountManager = Path.Combine(DefaultRelativePathDirectorySoftware, "simplewallet.exe");
        public static readonly string DefaultPathDirectoryAccountBackups = Path.Combine(DefaultRelativePathDirectoryAccountData, "Backups");
        public static readonly string DefaultPathFileAccountData = Path.Combine(DefaultRelativePathDirectoryAccountData, "account.bin");

        internal const int TimerSettingRpcCheckAvailabilityDueTime = 3000;
        internal const int TimerSettingRpcCheckAvailabilityPeriod = 1000;

        internal static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

        internal static string GetAbsolutePath(string input)
        {
            return new FileInfo(input).FullName;
        }

        internal static bool IsHostLocal(string input)
        {
            var uri = new Uri(input);
            IPAddress[] host;

            try {
                host = Dns.GetHostAddresses(uri.Host);
            } catch (Exception) {
                return false;
            }

            var hostIps = Dns.GetHostAddresses(Dns.GetHostName());

            return host.Any(hostAddress => IPAddress.IsLoopback(hostAddress) || hostIps.Contains(hostAddress));
        }

        internal static bool IsPortInUse(int port)
        {
            var activeTcpListeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
            for (var i = activeTcpListeners.Length - 1; i >= 0; i--) {
                if (activeTcpListeners[i].Port == port) {
                    return true;
                }
            }

            return false;
        }
    }
}
