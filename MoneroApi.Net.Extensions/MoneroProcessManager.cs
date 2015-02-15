using Jojatekok.MoneroAPI.Extensions.ProcessManagers;
using Jojatekok.MoneroAPI.Extensions.Settings;
using Jojatekok.MoneroAPI.Settings;
using System;

namespace Jojatekok.MoneroAPI.Extensions
{
    public class MoneroProcessManager : IDisposable
    {
        private IRpcSettings RpcSettings { get; set; }
        private IDaemonProcessSettings DaemonProcessSettings { get; set; }
        private IAccountManagerProcessSettings AccountManagerProcessSettings { get; set; }

        /// <summary>Contains methods to interact with the daemon process.</summary>
        public DaemonProcessManager Daemon { get; private set; }
        /// <summary>Contains methods to interact with the account manager process.</summary>
        public AccountProcessManager AccountManager { get; private set; }

        /// <summary>Creates a new instance of Monero API .NET's process manager service.</summary>
        /// <param name="rpcSettings">IP-related settings to use when communicating through the Monero core assemblies' RPC protocol.</param>
        /// <param name="accountManagerProcessSettings">Path settings for the account manager process.</param>
        /// <param name="daemonProcessSettings">Path settings for the daemon process.</param>
        public MoneroProcessManager(IRpcSettings rpcSettings, IAccountManagerProcessSettings accountManagerProcessSettings, IDaemonProcessSettings daemonProcessSettings)
        {
            if (rpcSettings == null) rpcSettings = new RpcSettings();
            if (daemonProcessSettings == null) daemonProcessSettings = new DaemonProcessSettings();
            if (accountManagerProcessSettings == null) accountManagerProcessSettings = new AccountManagerProcessSettings();

            RpcSettings = rpcSettings;
            DaemonProcessSettings = daemonProcessSettings;
            AccountManagerProcessSettings = accountManagerProcessSettings;

            Daemon = new DaemonProcessManager(rpcSettings, daemonProcessSettings);
            AccountManager = new AccountProcessManager(rpcSettings, accountManagerProcessSettings, Daemon);
        }

        /// <summary>Creates a new instance of Monero API .NET's process manager service.</summary>
        /// <param name="rpcSettings">IP-related settings to use when communicating through the Monero core assemblies' RPC protocol.</param>
        /// <param name="accountManagerProcessSettings">Path settings for the account manager process.</param>
        public MoneroProcessManager(IRpcSettings rpcSettings, IAccountManagerProcessSettings accountManagerProcessSettings) : this(rpcSettings, accountManagerProcessSettings, null)
        {

        }

        /// <summary>Creates a new instance of Monero API .NET's process manager service.</summary>
        /// <param name="rpcSettings">IP-related settings to use when communicating through the Monero core assemblies' RPC protocol.</param>
        public MoneroProcessManager(IRpcSettings rpcSettings) : this(rpcSettings, null, null)
        {

        }

        /// <summary>Creates a new instance of Monero API .NET's process manager service.</summary>
        public MoneroProcessManager() : this(null, null, null)
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
