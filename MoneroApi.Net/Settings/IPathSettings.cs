namespace Jojatekok.MoneroAPI.Settings
{
    public interface IPathSettings
    {
        string DirectoryDaemonData { get; set; }

        string DirectoryAccountData { get; }
        string DirectoryAccountBackups { get; set; }

        string FileAccountData { get; set; }
        string FileAccountDataKeys { get; }

        string SoftwareDaemon { get; set; }
        string SoftwareAccountManager { get; set; }

        bool StartDaemonProcess { get; set; }
        bool StartAccountManagerProcess { get; set; }
    }
}
