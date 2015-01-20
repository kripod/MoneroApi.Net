using Jojatekok.MoneroAPI.ProcessManagers;
using System;
using System.IO;

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
    }
}
