using Jojatekok.MoneroAPI.ProcessManagers;
using Jojatekok.MoneroAPI.RpcManagers;
using Jojatekok.MoneroAPI.Settings;
using System;

namespace Jojatekok.MoneroAPI
{
    public class MoneroClient : IDisposable
    {
        private RpcWebClient RpcWebClient { get; set; }
        private PathSettings PathSettings { get; set; }
        public TimerSettings TimerSettings { get; private set; }

        public DaemonManager Daemon { get; private set; }
        public AccountManager AccountManager { get; private set; }

        public MoneroClient(PathSettings pathSettings = null, RpcSettings rpcSettings = null, TimerSettings timerSettings = null)
        {
            if (pathSettings == null) pathSettings = new PathSettings();
            if (rpcSettings == null) rpcSettings = new RpcSettings();
            if (timerSettings == null) timerSettings = new TimerSettings();

            RpcWebClient = new RpcWebClient(rpcSettings);
            PathSettings = pathSettings;
            TimerSettings = timerSettings;

            Daemon = new DaemonManager(RpcWebClient, pathSettings, timerSettings);
            AccountManager = new AccountManager(RpcWebClient, pathSettings, timerSettings, Daemon);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing) {
                if (AccountManager != null) {
                    AccountManager.Dispose();
                    AccountManager = null;
                }

                if (Daemon != null) {
                    Daemon.Dispose();
                    Daemon = null;
                }
            }
        }
    }
}
