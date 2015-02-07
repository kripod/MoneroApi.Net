using Jojatekok.MoneroAPI.Extensions.ProcessManagers;
using Jojatekok.MoneroAPI.Extensions.Settings;
using Jojatekok.MoneroAPI.Settings;
using System;

namespace Jojatekok.MoneroAPI.Extensions
{
    public class MoneroProcessManager : IDisposable
    {
        private IRpcSettings RpcSettings { get; set; }
        private IDaemonPathSettings DaemonPathSettings { get; set; }
        private IAccountManagerPathSettings AccountManagerPathSettings { get; set; }

        /// <summary>Contains methods to interact with the daemon process.</summary>
        public DaemonProcessManager Daemon { get; private set; }
        /// <summary>Contains methods to interact with the account manager process.</summary>
        public AccountProcessManager AccountManager { get; private set; }

        /// <summary>Creates a new instance of Monero API .NET's process manager service.</summary>
        /// <param name="rpcSettings">IP-related settings to use when communicating through the Monero core assemblies' RPC protocol.</param>
        /// <param name="accountManagerPathSettings">Path settings for the account manager process.</param>
        /// <param name="daemonPathSettings">Path settings for the daemon process.</param>
        public MoneroProcessManager(IRpcSettings rpcSettings, IAccountManagerPathSettings accountManagerPathSettings, IDaemonPathSettings daemonPathSettings)
        {
            if (rpcSettings == null) rpcSettings = new RpcSettings();
            if (DaemonPathSettings == null) daemonPathSettings = new DaemonPathSettings();
            if (AccountManagerPathSettings == null) accountManagerPathSettings = new AccountManagerPathSettings();

            RpcSettings = rpcSettings;
            DaemonPathSettings = daemonPathSettings;
            AccountManagerPathSettings = accountManagerPathSettings;

            Daemon = new DaemonProcessManager(rpcSettings, daemonPathSettings);
            AccountManager = new AccountProcessManager(rpcSettings, accountManagerPathSettings, Daemon);
        }

        /// <summary>Creates a new instance of Monero API .NET's process manager service.</summary>
        /// <param name="rpcSettings">IP-related settings to use when communicating through the Monero core assemblies' RPC protocol.</param>
        /// <param name="accountManagerPathSettings">Path settings for the account manager process.</param>
        public MoneroProcessManager(IRpcSettings rpcSettings, IAccountManagerPathSettings accountManagerPathSettings) : this(rpcSettings, accountManagerPathSettings, null)
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
                if (Daemon != null) {
                    Daemon.Dispose();
                    Daemon = null;
                }

                if (AccountManager != null) {
                    AccountManager.Dispose();
                    AccountManager = null;
                }
            }
        }
    }
}
