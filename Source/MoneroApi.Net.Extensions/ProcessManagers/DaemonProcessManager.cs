using Jojatekok.MoneroAPI.Extensions.Settings;
using Jojatekok.MoneroAPI.Settings;
using System;
using System.Collections.Generic;

namespace Jojatekok.MoneroAPI.Extensions.ProcessManagers
{
    public class DaemonProcessManager : BaseRpcProcessManager
    {
        private List<string> ProcessArguments { get; set; }

        internal DaemonProcessManager(IRpcSettings rpcSettings, IDaemonProcessSettings processSettings) : base(processSettings.SoftwareDaemon, rpcSettings.UrlHostDaemon, rpcSettings.UrlPortDaemon)
        {
            ProcessArguments = new List<string> {
                "--log-level " + (int)processSettings.LogLevel
            };

            string testnetString;
            if (processSettings.UseTestnet) {
                testnetString = "testnet-";
                ProcessArguments.Add("--testnet");
            } else {
                testnetString = "";
            }

            if (rpcSettings.UrlHostDaemon != MoneroAPI.Utilities.DefaultRpcUrlHost) {
                ProcessArguments.Add("--rpc-bind-ip " + rpcSettings.UrlHostDaemon);
            }

            ProcessArguments.Add("--" + testnetString + "rpc-bind-port " + rpcSettings.UrlPortDaemon);
            ProcessArguments.Add("--" + testnetString + "data-dir \"" + processSettings.DirectoryDaemonData + "\"");
        }

        public void Start()
        {
            StartProcess(ProcessArguments);
        }

        public void Stop()
        {
            KillBaseProcess();
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        public void DisposeSafely()
        {
            // Save the blockchain before disposing
            SendConsoleCommand("exit");
            IsDisposeSafe = true;

            Dispose();
        }

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing) {
                base.Dispose();
            }
        }
    }
}
