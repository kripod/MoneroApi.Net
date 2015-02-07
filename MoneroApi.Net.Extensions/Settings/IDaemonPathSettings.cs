namespace Jojatekok.MoneroAPI.Extensions.Settings
{
    public interface IDaemonPathSettings
    {
        string SoftwareDaemon { get; set; }

        string DirectoryDaemonData { get; set; }
    }
}
