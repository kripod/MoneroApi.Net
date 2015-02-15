namespace Jojatekok.MoneroAPI.Extensions.Settings
{
    public class DaemonProcessSettings : IDaemonProcessSettings
    {
        private string _softwareDaemon = Utilities.DefaultPathSoftwareDaemon;
        private string _directoryDaemonData = Utilities.DefaultPathDirectoryDaemonData;

        public LogLevel LogLevel { get; set; }

        public bool UseTestnet { get; set; }

        public string SoftwareDaemon {
            get { return Utilities.GetAbsolutePath(_softwareDaemon); }
            set { _softwareDaemon = value; }
        }

        public string DirectoryDaemonData {
            get { return Utilities.GetAbsolutePath(_directoryDaemonData); }
            set { _directoryDaemonData = value; }
        }
    }
}
