namespace Jojatekok.MoneroAPI.Extensions.Settings
{
    public class AccountManagerProcessSettings : IAccountManagerProcessSettings
    {
        private string _softwareAccountManager = Utilities.DefaultPathSoftwareAccountManager;
        private string _directoryAccountBackups = Utilities.DefaultPathDirectoryAccountBackups;
        private string _fileAccountData = Utilities.DefaultPathFileAccountData;

        public LogLevel LogLevel { get; set; }

        public string SoftwareAccountManager {
            get { return Utilities.GetAbsolutePath(_softwareAccountManager); }
            set { _softwareAccountManager = value; }
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
    }
}
