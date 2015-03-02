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

        public static readonly string DefaultPathSoftwareDaemon = Path.Combine(DefaultRelativePathDirectorySoftware, "bitmonerod" + FileExtensionExecutable);
        public static readonly string DefaultPathDirectoryDaemonData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "bitmonero");

        public static readonly string DefaultPathSoftwareAccountManager = Path.Combine(DefaultRelativePathDirectorySoftware, "simplewallet" + FileExtensionExecutable);
        public static readonly string DefaultPathDirectoryAccountBackups = Path.Combine(DefaultRelativePathDirectoryAccountData, "Backups");
        public static readonly string DefaultPathFileAccountData = Path.Combine(DefaultRelativePathDirectoryAccountData, "account.bin");

        internal const int TimerSettingRpcCheckAvailabilityDueTime = 3000;
        internal const int TimerSettingRpcCheckAvailabilityPeriod = 1000;

        internal static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

        private static readonly string FileExtensionExecutable = IsOsUnix() ? "" : ".exe";

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

        private static bool IsOsUnix()
        {
            var platform = (int)Environment.OSVersion.Platform;
            if ((platform == 4) || (platform == 6) || (platform == 128)) {
                return true;
            } else {
                return false;
            }
        }
    }
}
