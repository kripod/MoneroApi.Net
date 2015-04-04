using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Jojatekok.MoneroAPI.Extensions.ProcessManagers
{
    public abstract class BaseRpcProcessManager : IDisposable
    {
        public event EventHandler Initialized;
        public event EventHandler<LogMessageReceivedEventArgs> OnLogMessage;

        protected event EventHandler<ProcessExitedEventArgs> Exited;

        private bool _isRpcAvailable;

        private bool IsInitialized { get; set; }

        private string Path { get; set; }
        private Process Process { get; set; }

        private ushort RpcPort { get; set; }

        private Timer TimerCheckRpcAvailability { get; set; }

        internal bool IsRpcAvailable {
            get { return _isRpcAvailable; }

            private set {
                if (value == _isRpcAvailable) return;

                _isRpcAvailable = value;
                if (!value) return;

                if (TimerCheckRpcAvailability != null) TimerCheckRpcAvailability.Stop();

                if (!IsInitialized) {
                    IsInitialized = true;
                    if (Initialized != null) Initialized(this, EventArgs.Empty);
                }
            }
        }

        protected bool IsDisposeSafe { get; set; }
        private bool IsDisposing { get; set; }

        private bool IsProcessAlive {
            get { return Process != null && !Process.HasExited; }
        }

        internal BaseRpcProcessManager(string path, string rpcHost, ushort rpcPort) {
            if (Utilities.IsHostLocal(rpcHost)) {
                TimerCheckRpcAvailability = new Timer(delegate { CheckRpcAvailability(); });
            } else {
                IsRpcAvailable = true;
            }

            Path = path;
            RpcPort = rpcPort;
        }

        protected void StartProcess(IEnumerable<string> arguments)
        {
            if (Process != null) Process.Dispose();

            Process = new Process {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo(Path) {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true
                }
            };

            if (arguments != null) {
                Process.StartInfo.Arguments = string.Join(" ", arguments);
            }

            Process.OutputDataReceived += OnProcessOutputDataReceived;
            Process.Exited += OnProcessExited;

            Process.Start();
            Process.BeginOutputReadLine();

            if (TimerCheckRpcAvailability != null) {
                // Constantly check for the RPC port's activeness
                TimerCheckRpcAvailability.Change(Utilities.TimerSettingRpcCheckAvailabilityDueTime, Utilities.TimerSettingRpcCheckAvailabilityPeriod);
            }
        }

        protected void StopProcess()
        {
            if (Process == null) return;

            try {
                if (!Process.HasExited) {
                    if (IsDisposeSafe) {
                        Process.WaitForExit();
                    } else {
                        Process.Kill();
                    }
                }
// ReSharper disable once EmptyGeneralCatchClause
            } catch {
                
            }

            Process.Dispose();
            Process = null;
        }

        private void CheckRpcAvailability()
        {
            IsRpcAvailable = Utilities.IsPortInUse(RpcPort);
        }

        public void SendConsoleCommand(string input)
        {
            if (IsProcessAlive) {
                if (OnLogMessage != null) OnLogMessage(this, new LogMessageReceivedEventArgs(new LogMessage("> " + input, DateTime.UtcNow)));
                Process.StandardInput.WriteLine(input);
            }
        }

        protected void KillBaseProcess()
        {
            if (IsProcessAlive) {
                Process.Kill();
            }
        }

        private void OnProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            var line = e.Data;
            if (line == null) return;

            if (OnLogMessage != null) OnLogMessage(this, new LogMessageReceivedEventArgs(new LogMessage(line, DateTime.UtcNow)));
        }

        private void OnProcessExited(object sender, EventArgs e)
        {
            if (IsDisposing) return;

            IsRpcAvailable = false;
            Process.CancelOutputRead();
            if (Exited != null) Exited(this, new ProcessExitedEventArgs(Process.ExitCode));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !IsDisposing) {
                IsDisposing = true;

                if (TimerCheckRpcAvailability != null) {
                    TimerCheckRpcAvailability.Dispose();
                    TimerCheckRpcAvailability = null;
                }

                StopProcess();
            }
        }
    }
}
