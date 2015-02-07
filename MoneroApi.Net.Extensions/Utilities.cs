using Jojatekok.MoneroAPI.Extensions.ProcessManagers;
using Jojatekok.MoneroAPI.Extensions.Settings;
using Jojatekok.MoneroAPI.ProcessManagers;
using Jojatekok.MoneroAPI.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.NetworkInformation;

namespace Jojatekok.MoneroAPI.Extensions
{
    public static class Utilities
    {
        private const string DefaultRelativePathDirectorySoftware = "Resources\\Software\\";
        private const string DefaultRelativePathDirectoryAccountData = "AccountData\\";

        public const string DefaultPathSoftwareDaemon = DefaultRelativePathDirectorySoftware + "bitmonerod.exe";
        public static readonly string DefaultPathDirectoryDaemonData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "bitmonero");

        public const string DefaultPathSoftwareAccountManager = DefaultRelativePathDirectorySoftware + "simplewallet.exe";
        public const string DefaultPathDirectoryAccountBackups = DefaultRelativePathDirectoryAccountData + "Backups\\";
        public const string DefaultPathFileAccountData = DefaultRelativePathDirectoryAccountData + "account.bin";

        internal const int TimerSettingRpcCheckAvailabilityDueTime = 3000;
        internal const int TimerSettingRpcCheckAvailabilityPeriod = 1000;

        internal static readonly JobManager JobManager = new JobManager();

        internal static readonly string ApplicationDirectory = AppDomain.CurrentDomain.BaseDirectory;

        internal static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

        public static void StartProcess(this IDaemonManager daemon, IDaemonPathSettings pathSettings, IRpcSettings rpcSettings)
        {
            var processArguments = new List<string> {
                "--log-level 0",
                "--data-dir \"" + pathSettings.DirectoryDaemonData + "\"",
                "--rpc-bind-port " + rpcSettings.UrlPortDaemon
            };

            if (rpcSettings.UrlHostDaemon != MoneroAPI.Utilities.DefaultRpcUrlHost) {
                processArguments.Add("--rpc-bind-ip " + rpcSettings.UrlHostDaemon);
            }

            ProcessManager.StartProcess(pathSettings.SoftwareDaemon, processArguments, rpcSettings.UrlHostDaemon, rpcSettings.UrlPortDaemon);
        }

        internal static string GetAbsolutePath(string input)
        {
            return input.Contains(":") ? input : Path.GetFullPath(Path.Combine(ApplicationDirectory, input));
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
