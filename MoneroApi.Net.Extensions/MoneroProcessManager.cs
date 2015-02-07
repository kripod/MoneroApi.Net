using Jojatekok.MoneroAPI.Extensions.Settings;
using Jojatekok.MoneroAPI.ProcessManagers;
using Jojatekok.MoneroAPI.RpcManagers;
using Jojatekok.MoneroAPI.Settings;
using System;

namespace Jojatekok.MoneroAPI.Extensions
{
    public class MoneroProcessManager : IDisposable
    {
        private IDaemonPathSettings DaemonPathSettings { get; set; }
        private IAccountManagerPathSettings AccountManagerPathSettings { get; set; }

        /// <summary>Creates a new instance of Monero API .NET's process manager service.</summary>
        /// <param name="accountManagerPathSettings">Path settings for the account manager process.</param>
        /// <param name="daemonManagerPathSettings">Path settings for the daemon process.</param>
        public MoneroProcessManager(IAccountManagerPathSettings accountManagerPathSettings, IDaemonPathSettings daemonManagerPathSettings)
        {
            DaemonPathSettings = DaemonPathSettings;
            AccountManagerPathSettings = accountManagerPathSettings;
        }

        /// <summary>Creates a new instance of Monero API .NET's process manager service.</summary>
        /// <param name="accountManagerPathSettings">Path settings for the account manager process.</param>
        public MoneroProcessManager(IAccountManagerPathSettings accountManagerPathSettings) : this(accountManagerPathSettings, new DaemonPathSettings())
        {

        }

        /// <summary>Creates a new instance of Monero API .NET's process manager service.</summary>
        public MoneroProcessManager() : this(null, null)
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
                // TODO
            }
        }
    }
}
