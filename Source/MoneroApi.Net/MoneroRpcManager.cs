using Jojatekok.MoneroAPI.RpcManagers;
using Jojatekok.MoneroAPI.RpcUtilities;
using Jojatekok.MoneroAPI.Settings;
using System;

namespace Jojatekok.MoneroAPI
{
    public class MoneroRpcManager : IDisposable
    {
        private RpcWebClient RpcWebClient { get; set; }

        private IRpcSettings RpcSettings { get; set; }
        /// <summary>Represents the settings of data fetching timers.</summary>
        public ITimerSettings TimerSettings { get; private set; }

        /// <summary>Contains methods to interact with the daemon's RPC service.</summary>
        public IDaemonRpcManager Daemon { get; private set; }
        /// <summary>Contains methods to interact with the account manager's RPC service.</summary>
        public IAccountRpcManager AccountManager { get; private set; }

        private bool IsDisposeSafe { get; set; }

        /// <summary>Creates a new instance of Monero API .NET's RPC manager service.</summary>
        /// <param name="rpcSettings">IP-related settings to use when communicating through the Monero core assemblies' RPC protocol.</param>
        /// <param name="timerSettings">Timer settings which can be used to alter data fetching intervals.</param>
        public MoneroRpcManager(IRpcSettings rpcSettings, ITimerSettings timerSettings)
        {
            if (rpcSettings == null) rpcSettings = new RpcSettings();
            if (timerSettings == null) timerSettings = new TimerSettings();

            RpcWebClient = new RpcWebClient(rpcSettings, timerSettings);

            RpcSettings = rpcSettings;
            TimerSettings = timerSettings;

            Daemon = new DaemonRpcManager(RpcWebClient);
            AccountManager = new AccountRpcManager(RpcWebClient);
        }

        /// <summary>Creates a new instance of Monero API .NET's RPC manager service.</summary>
        /// <param name="rpcSettings">IP-related settings to use when communicating through the Monero core assemblies' RPC protocol.</param>
        public MoneroRpcManager(IRpcSettings rpcSettings) : this(rpcSettings, null)
        {

        }

        /// <summary>Creates a new instance of Monero API .NET's RPC manager service.</summary>
        public MoneroRpcManager() : this(null, null)
        {

        }

        /// <summary>Saves the account file, and then disposes the RPC manager services.</summary>
        public void DisposeSafely()
        {
            IsDisposeSafe = true;
            Dispose();
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
                    if (IsDisposeSafe) {
                        AccountManager.DisposeSafely();
                    } else {
                        AccountManager.Dispose();
                    }
                    AccountManager = null;
                }

                RpcWebClient.IsEnabled = false;

                if (Daemon != null) {
                    Daemon.Dispose();
                    Daemon = null;
                }
            }
        }
    }
}
