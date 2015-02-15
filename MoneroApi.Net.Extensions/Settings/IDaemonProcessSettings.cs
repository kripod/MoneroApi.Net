namespace Jojatekok.MoneroAPI.Extensions.Settings
{
    public interface IDaemonProcessSettings
    {
        LogLevel LogLevel { get; set; }

        string SoftwareDaemon { get; set; }

        string DirectoryDaemonData { get; set; }
    }
}
