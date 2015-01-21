using Jojatekok.MoneroAPI.ProcessManagers;
using System;
using System.Globalization;
using System.IO;
using System.Net.NetworkInformation;

namespace Jojatekok.MoneroAPI
{
    static class Utilities
    {
        private const string DefaultRelativePathDirectoryAccountData = "AccountData\\";
        private const string DefaultRelativePathDirectorySoftware = "Resources\\Software\\";

        public static readonly string DefaultPathDirectoryDaemonData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "bitmonero");
        public const string DefaultPathDirectoryAccountBackups = DefaultRelativePathDirectoryAccountData + "Backups\\";
        public const string DefaultPathFileAccountData = DefaultRelativePathDirectoryAccountData + "account.bin";
        public const string DefaultPathSoftwareDaemon = DefaultRelativePathDirectorySoftware + "bitmonerod.exe";
        public const string DefaultPathSoftwareAccountManager = DefaultRelativePathDirectorySoftware + "simplewallet.exe";

        public const string DefaultRpcUrlHostDaemon = "http://localhost";
        public const string DefaultRpcUrlHostAccountManager = "http://localhost";
        public const ushort DefaultRpcUrlPortDaemon = 18081;
        public const ushort DefaultRpcUrlPortAccountManager = 18082;

        public static readonly string ApplicationDirectory = AppDomain.CurrentDomain.BaseDirectory;

        public static readonly JobManager JobManager = new JobManager();

        public static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

        public static string GetAbsolutePath(string input)
        {
            return input.Contains(":") ? input : Path.GetFullPath(Path.Combine(ApplicationDirectory, input));
        }

        public static bool IsPortInUse(int port)
        {
            var activeTcpListeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
            for (var i = activeTcpListeners.Length - 1; i >= 0; i--) {
                if (activeTcpListeners[i].Port == port) {
                    return true;
                }
            }

            return false;
        }

        public static DateTime UnixTimestampToDateTime(ulong unixTimeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp);
        }
    }
}
