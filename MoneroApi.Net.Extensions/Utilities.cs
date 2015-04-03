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
        private static EnvironmentPlatform? _environmentPlatform;
        private static string _environmentDocumentsDirectory;

        private static readonly string EnvironmentFileExtensionExecutable = EnvironmentPlatform == EnvironmentPlatform.Windows ? ".exe" : "";

        private static readonly string DefaultRelativePathDirectorySoftware = Path.Combine("Resources", "Software");

        public static readonly string DefaultPathSoftwareDaemon = Path.Combine(DefaultRelativePathDirectorySoftware, "bitmonerod" + EnvironmentFileExtensionExecutable);
        public static readonly string DefaultPathDirectoryDaemonData = EnvironmentPlatform == EnvironmentPlatform.Windows ?
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "bitmonero") :
            "~/.bitmonero";

        public static readonly string DefaultPathSoftwareAccountManager = Path.Combine(DefaultRelativePathDirectorySoftware, "simplewallet" + EnvironmentFileExtensionExecutable);
        public static readonly string DefaultPathDirectoryAccountData = Path.Combine(EnvironmentDocumentsDirectory, "Monero Accounts");
        public static readonly string DefaultPathDirectoryAccountBackups = Path.Combine(DefaultPathDirectoryAccountData, "Backups");
        public static readonly string DefaultPathFileAccountData = Path.Combine(DefaultPathDirectoryAccountData, "account.bin");

        internal const int TimerSettingRpcCheckAvailabilityDueTime = 3000;
        internal const int TimerSettingRpcCheckAvailabilityPeriod = 1000;

        internal static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

        public static EnvironmentPlatform EnvironmentPlatform {
            get {
                if (_environmentPlatform == null) {
                    switch (Environment.OSVersion.Platform) {
                        case PlatformID.Unix:
                            if (Directory.Exists("/Applications") & Directory.Exists("/System") & Directory.Exists("/Users") & Directory.Exists("/Volumes")) {
                                _environmentPlatform = EnvironmentPlatform.Mac;
                            }
                            _environmentPlatform = EnvironmentPlatform.Linux;
                            break;

                        case PlatformID.MacOSX:
                            _environmentPlatform = EnvironmentPlatform.Mac;
                            break;

                        default:
                            _environmentPlatform = EnvironmentPlatform.Windows;
                            break;
                    }
                }

                return _environmentPlatform.Value;
            }
        }

        private static string EnvironmentDocumentsDirectory {
            get {
                if (_environmentDocumentsDirectory == null) {
                    switch (EnvironmentPlatform) {
                        case EnvironmentPlatform.Windows:
                            _environmentDocumentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                            break;

                        case EnvironmentPlatform.Mac:
                            _environmentDocumentsDirectory = "~/Documents";
                            break;

                        default:
                            _environmentDocumentsDirectory = "~";
                            break;
                    }
                }

                return _environmentDocumentsDirectory;
            }
        }

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
