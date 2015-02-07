namespace Jojatekok.MoneroAPI.Extensions.Settings
{
    public interface IAccountManagerPathSettings
    {
        string SoftwareAccountManager { get; set; }

        string DirectoryAccountData { get; }
        string DirectoryAccountBackups { get; set; }

        string FileAccountData { get; set; }
        string FileAccountDataKeys { get; }
    }
}
