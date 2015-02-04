using Jojatekok.MoneroAPI.ProcessManagers;
using Jojatekok.MoneroAPI.RpcManagers;
using Jojatekok.MoneroAPI.Settings;
using System;

namespace Jojatekok.MoneroAPI
{
    public class MoneroClient : IDisposable
    {
        private RpcWebClient RpcWebClient { get; set; }

        private IRpcSettings RpcSettings { get; set; }
        /// <summary>Represents the settings of data fetching timers.</summary>
        public ITimerSettings TimerSettings { get; private set; }

        /// <summary>Contains methods to interact with the daemon.</summary>
        public DaemonManager Daemon { get; private set; }
        /// <summary>Contains methods to interact with the account manager.</summary>
        public IAccountManager AccountManager { get; private set; }

        /// <summary>Creates a new instance of Monero API .NET's client service.</summary>
        /// <param name="rpcSettings">IP-related settings to use when communicating through the Monero core assemblies' RPC protocol.</param>
        /// <param name="timerSettings">Timer settings which can be used to alter data fetching intervals.</param>
        public MoneroClient(IRpcSettings rpcSettings, ITimerSettings timerSettings)
        {
            if (rpcSettings == null) rpcSettings = new RpcSettings();
            if (timerSettings == null) timerSettings = new TimerSettings();

            RpcWebClient = new RpcWebClient(rpcSettings, timerSettings);

            RpcSettings = rpcSettings;
            TimerSettings = timerSettings;

            Daemon = new DaemonManager(RpcWebClient);
            AccountManager = new AccountManager(RpcWebClient, Daemon);
        }

        /// <summary>Creates a new instance of Monero API .NET's client service.</summary>
        /// <param name="rpcSettings">IP-related settings to use when communicating through the Monero core assemblies' RPC protocol.</param>
        public MoneroClient(IRpcSettings rpcSettings) : this(rpcSettings, null)
        {

        }

        /// <summary>Creates a new instance of Monero API .NET's client service.</summary>
        public MoneroClient() : this(null, null)
        {

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
