namespace Jojatekok.MoneroAPI.Settings
{
    public class PathSettings
    {
        private string _directoryDaemonData = Utilities.DefaultPathDirectoryDaemonData;
        public string DirectoryDaemonData {
            get { return Helper.GetAbsolutePath(_directoryDaemonData); }
            set { _directoryDaemonData = value; }
        }

        public string DirectoryAccountData {
            get {
                var lastIndexOfSlash = FileAccountData.LastIndexOf('\\');
                return lastIndexOfSlash >= 0 ? Helper.GetAbsolutePath(FileAccountData.Substring(0, FileAccountData.LastIndexOf('\\'))) : Utilities.ApplicationDirectory;
            }
        }

        private string _directoryAccountBackups = Utilities.DefaultPathDirectoryAccountBackups;
        public string DirectoryAccountBackups {
            get { return Helper.GetAbsolutePath(_directoryAccountBackups); }
            set { _directoryAccountBackups = value; }
        }

        private string _fileAccountData = Utilities.DefaultPathFileAccountData;
        public string FileAccountData {
            get { return Helper.GetAbsolutePath(_fileAccountData); }
            set { _fileAccountData = value; }
        }

        public string FileAccountDataKeys {
            get { return Helper.GetAbsolutePath(FileAccountData + ".keys"); }
        }

        private string _softwareDaemon = Utilities.DefaultPathSoftwareDaemon;
        public string SoftwareDaemon {
            get { return Helper.GetAbsolutePath(_softwareDaemon); }
            set { _softwareDaemon = value; }
        }

        private string _softwareAccountManager = Utilities.DefaultPathSoftwareAccountManager;
        public string SoftwareAccountManager {
            get { return Helper.GetAbsolutePath(_softwareAccountManager); }
            set { _softwareAccountManager = value; }
        }
    }
}
