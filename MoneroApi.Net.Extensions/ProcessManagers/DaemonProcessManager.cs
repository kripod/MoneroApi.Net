using Jojatekok.MoneroAPI.Extensions.Settings;
using Jojatekok.MoneroAPI.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jojatekok.MoneroAPI.Extensions.ProcessManagers
{
    public class DaemonProcessManager : BaseRpcProcessManager
    {
        private static readonly string[] ProcessArgumentsDefault = { "--log-level 0" };
        private List<string> ProcessArgumentsExtra { get; set; }

        internal DaemonProcessManager(IRpcSettings rpcSettings, IDaemonPathSettings pathSettings) : base(pathSettings.SoftwareDaemon, rpcSettings.UrlHostDaemon, rpcSettings.UrlPortDaemon)
        {
            ProcessArgumentsExtra = new List<string> {
                //"--data-dir \"" + paths.DirectoryDaemonData + "\"",
                "--rpc-bind-port " + rpcSettings.UrlPortDaemon
            };

            if (rpcSettings.UrlHostDaemon != MoneroAPI.Utilities.DefaultRpcUrlHost) {
                ProcessArgumentsExtra.Add("--rpc-bind-ip " + rpcSettings.UrlHostDaemon);
            }

            // TODO: Remove this temporary fix
            ProcessArgumentsExtra.Add("--data-dir \"" + pathSettings.DirectoryDaemonData);
        }

        public void Start()
        {
            StartProcess(ProcessArgumentsDefault.Concat(ProcessArgumentsExtra));
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

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing) {
                // Safe shutdown
                SendConsoleCommand("exit");
                IsDisposeProcessKillNecessary = false;

                base.Dispose();
            }
        }
    }
}
