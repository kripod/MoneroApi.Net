namespace Jojatekok.MoneroAPI.Settings
{
    public interface ITimerSettings
    {
        int DaemonQueryNetworkInformationPeriod { get; set; }

        int AccountRefreshPeriod { get; set; }
    }
}
