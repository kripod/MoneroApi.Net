namespace Jojatekok.MoneroAPI.Settings
{
    public class PathSettings : IPathSettings
    {
        private string _directoryDaemonData = Utilities.DefaultPathDirectoryDaemonData;
        private string _directoryAccountBackups = Utilities.DefaultPathDirectoryAccountBackups;
        private string _fileAccountData = Utilities.DefaultPathFileAccountData;
        private string _softwareDaemon = Utilities.DefaultPathSoftwareDaemon;
        private string _softwareAccountManager = Utilities.DefaultPathSoftwareAccountManager;

        public string DirectoryDaemonData {
            get { return Utilities.GetAbsolutePath(_directoryDaemonData); }
            set { _directoryDaemonData = value; }
        }

        public string DirectoryAccountData {
            get {
                var lastIndexOfSlash = FileAccountData.LastIndexOf('\\');
                return lastIndexOfSlash >= 0 ? Utilities.GetAbsolutePath(FileAccountData.Substring(0, FileAccountData.LastIndexOf('\\'))) : Utilities.ApplicationDirectory;
            }
        }

        public string DirectoryAccountBackups {
            get { return Utilities.GetAbsolutePath(_directoryAccountBackups); }
            set { _directoryAccountBackups = value; }
        }

        public string FileAccountData {
            get { return Utilities.GetAbsolutePath(_fileAccountData); }
            set { _fileAccountData = value; }
        }

        public string FileAccountDataKeys {
            get { return Utilities.GetAbsolutePath(FileAccountData + ".keys"); }
        }

        public string SoftwareDaemon {
            get { return Utilities.GetAbsolutePath(_softwareDaemon); }
            set { _softwareDaemon = value; }
        }

        public string SoftwareAccountManager {
            get { return Utilities.GetAbsolutePath(_softwareAccountManager); }
            set { _softwareAccountManager = value; }
        }
    }
}
