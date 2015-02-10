using Jojatekok.MoneroAPI.Extensions.Settings;
using Jojatekok.MoneroAPI.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Jojatekok.MoneroAPI.Extensions.ProcessManagers
{
    public class AccountProcessManager : BaseRpcProcessManager
    {
        public event EventHandler<PassphraseRequestedEventArgs> PassphraseRequested;

        private string _passphrase;

        private static readonly string[] ProcessArgumentsDefault = { "--set_log 0" };
        private List<string> ProcessArgumentsExtra { get; set; }

        private bool IsWaitingForStart { get; set; }
        private bool IsStartForced { get; set; }

        private IRpcSettings RpcSettings { get; set; }
        private IAccountManagerPathSettings PathSettings { get; set; }
        private DaemonProcessManager DaemonProcess { get; set; }

        public string Passphrase {
            get { return _passphrase; }

            set {
                _passphrase = value;
                Restart();
            }
        }

        private bool IsAccountKeysFileExistent {
            get { return File.Exists(PathSettings.FileAccountDataKeys); }
        }

        internal AccountProcessManager(IRpcSettings rpcSettings, IAccountManagerPathSettings pathSettings, DaemonProcessManager daemonProcess) : base(pathSettings.SoftwareAccountManager, rpcSettings.UrlHostAccountManager, rpcSettings.UrlPortAccountManager)
        {
            Exited += Process_Exited;

            RpcSettings = rpcSettings;
            PathSettings = pathSettings;
            DaemonProcess = daemonProcess;
        }

        private void SetProcessArguments()
        {
            ProcessArgumentsExtra = new List<string>(5) {
                "--daemon-address " + RpcSettings.UrlHostDaemon + ":" + RpcSettings.UrlPortDaemon,
                "--password \"" + Passphrase + "\""
            };

            if (IsAccountKeysFileExistent) {
                // Load existing account
                ProcessArgumentsExtra.Add("--wallet-file \"" + PathSettings.FileAccountData + "\"");

                if (RpcSettings.UrlHostAccountManager != MoneroAPI.Utilities.DefaultRpcUrlHost) {
                    ProcessArgumentsExtra.Add("--rpc-bind-ip " + RpcSettings.UrlHostAccountManager);
                }
                ProcessArgumentsExtra.Add("--rpc-bind-port " + RpcSettings.UrlPortAccountManager);

            } else {
                // Create new account
                var directoryAccountData = PathSettings.DirectoryAccountData;

                if (!Directory.Exists(directoryAccountData)) Directory.CreateDirectory(directoryAccountData);
                ProcessArgumentsExtra.Add("--generate-new-wallet \"" + PathSettings.FileAccountData + "\"");
            }
        }

        public void Start()
        {
            if (IsStartForced || IsAccountKeysFileExistent) {
                // Start the account normally
                IsStartForced = false;
                StartInternal();

            } else {
                // Let the user set a password for the new account being created
                IsStartForced = true;
                RequestPassphrase(true);
            }
        }

        private void StartInternal()
        {
            SetProcessArguments();

            if (!IsAccountKeysFileExistent) {
                OnLogMessage += AccountManager_OnLogMessage;
            }

            if (DaemonProcess.IsRpcAvailable) {
                StartProcess(ProcessArgumentsDefault.Concat(ProcessArgumentsExtra).ToArray());
            } else {
                IsWaitingForStart = true;
                DaemonProcess.Initialized += Daemon_Initialized;
            }
        }

        private void AccountManager_OnLogMessage(object sender, LogMessageReceivedEventArgs e)
        {
            var messageText = e.LogMessage.MessageText;

            // TODO: Allow selection of the deterministic seed's language
            if (messageText.StartsWith("0", StringComparison.Ordinal)) {
                SendConsoleCommand("0");
                return;
            }

            if (messageText.StartsWith("*", StringComparison.Ordinal)) {
                OnLogMessage -= AccountManager_OnLogMessage;
                Restart();
                return;
            }

            if (IsDisposing && messageText.Contains("data saved")) {
                OnLogMessage -= AccountManager_OnLogMessage;
                StopProcess();
            }
        }

        private void Daemon_Initialized(object sender, EventArgs e)
        {
            if (IsWaitingForStart) {
                IsWaitingForStart = false;
                StartProcess(ProcessArgumentsDefault.Concat(ProcessArgumentsExtra).ToArray());
            }
        }

        public void Stop()
        {
            KillBaseProcess();
        }

        public void Restart()
        {
            Stop();
            StartInternal();
        }

        private void RequestPassphrase(bool isFirstTime)
        {
            if (PassphraseRequested != null) PassphraseRequested(this, new PassphraseRequestedEventArgs(isFirstTime));
        }

        private string Backup(string path = null)
        {
            if (path == null) {
                path = PathSettings.DirectoryAccountBackups + DateTime.Now.ToString("yyyy-MM-dd", Utilities.InvariantCulture);
            }

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var accountName = Path.GetFileNameWithoutExtension(PathSettings.FileAccountData);

            var filesToBackup = Directory.GetFiles(PathSettings.DirectoryAccountData, accountName + "*", SearchOption.TopDirectoryOnly);
            for (var i = filesToBackup.Length - 1; i >= 0; i--) {
                var file = filesToBackup[i];
                Debug.Assert(file != null, "file != null");
                File.Copy(file, Path.Combine(path, Path.GetFileName(file)), true);
            }

            return path;
        }

        public Task<string> BackupAsync(string path)
        {
            return Task.Factory.StartNew(() => Backup(path));
        }

        public Task<string> BackupAsync()
        {
            return Task.Factory.StartNew(() => Backup());
        }

        private void Process_Exited(object sender, ProcessExitedEventArgs e)
        {
            switch (e.ExitCode) {
                case 1:
                    // Invalid passphrase
                    RequestPassphrase(false);
                    break;
            }
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
                OnLogMessage += AccountManager_OnLogMessage;
                SendConsoleCommand("save");
                IsDisposeProcessKillNecessary = false;

                base.Dispose();
            }
        }
    }
}
