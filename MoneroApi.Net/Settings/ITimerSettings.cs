namespace Jojatekok.MoneroAPI.Settings
{
    public interface ITimerSettings
    {
        int RpcCheckAvailabilityPeriod { get; set; }
        int RpcCheckAvailabilityDueTime { get; set; }

        int DaemonQueryNetworkInformationPeriod { get; set; }

        int AccountRefreshPeriod { get; set; }
    }
}
