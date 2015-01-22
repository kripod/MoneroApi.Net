namespace Jojatekok.MoneroAPI.Settings
{
    public class TimerSettings : ITimerSettings
    {
        private int _rpcCheckAvailabilityPeriod = Utilities.DefaultTimerSettingRpcCheckAvailabilityPeriod;
        private int _rpcCheckAvailabilityDueTime = Utilities.DefaultTimerSettingRpcCheckAvailabilityDueTime;
        private int _daemonQueryNetworkInformationPeriod = Utilities.DefaultTimerSettingDaemonQueryNetworkInformationPeriod;
        private int _accountRefreshPeriod = Utilities.DefaultTimerSettingAccountRefreshPeriod;

        public int RpcCheckAvailabilityPeriod {
            get { return _rpcCheckAvailabilityPeriod; }
            set { _rpcCheckAvailabilityPeriod = value; }
        }

        public int RpcCheckAvailabilityDueTime {
            get { return _rpcCheckAvailabilityDueTime; }
            set { _rpcCheckAvailabilityDueTime = value; }
        }

        public int DaemonQueryNetworkInformationPeriod {
            get { return _daemonQueryNetworkInformationPeriod; }
            set { _daemonQueryNetworkInformationPeriod = value; }
        }

        public int AccountRefreshPeriod {
            get { return _accountRefreshPeriod; }
            set { _accountRefreshPeriod = value; }
        }
    }
}
