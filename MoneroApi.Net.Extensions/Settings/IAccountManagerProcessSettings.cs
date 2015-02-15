namespace Jojatekok.MoneroAPI.Extensions.Settings
{
    public interface IAccountManagerProcessSettings
    {
        LogLevel LogLevel { get; set; }

        string SoftwareAccountManager { get; set; }

        string DirectoryAccountData { get; }
        string DirectoryAccountBackups { get; set; }

        string FileAccountData { get; set; }
        string FileAccountDataKeys { get; }
    }
}
