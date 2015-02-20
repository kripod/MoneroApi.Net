using Jojatekok.MoneroAPI.Extensions.Settings;
using Jojatekok.MoneroAPI.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Jojatekok.MoneroAPI.Extensions.ProcessManagers
{
    public class AccountProcessManager : BaseRpcProcessManager
    {
        public event EventHandler<PassphraseRequestedEventArgs> PassphraseRequested;

        private string _passphrase;

        private List<string> ProcessArguments { get; set; }

        private bool IsWaitingForStart { get; set; }
        private bool IsStartForced { get; set; }

        private IRpcSettings RpcSettings { get; set; }
        private IAccountManagerProcessSettings ProcessSettings { get; set; }
        private DaemonProcessManager DaemonProcess { get; set; }

        public string Passphrase {
            get { return _passphrase; }

            set {
                _passphrase = value;
                Restart();
            }
        }

        private bool IsAccountKeysFileExistent {
            get { return File.Exists(ProcessSettings.FileAccountDataKeys); }
        }

        internal AccountProcessManager(IRpcSettings rpcSettings, IAccountManagerProcessSettings pathSettings, DaemonProcessManager daemonProcess) : base(pathSettings.SoftwareAccountManager, rpcSettings.UrlHostAccountManager, rpcSettings.UrlPortAccountManager)
        {
            Exited += OnProcessExited;

            RpcSettings = rpcSettings;
            ProcessSettings = pathSettings;
            DaemonProcess = daemonProcess;
        }

        private void SetProcessArguments()
        {
            ProcessArguments = new List<string> {
                "--set_log " + (int)ProcessSettings.LogLevel,
                "--daemon-address " + RpcSettings.UrlHostDaemon + ":" + RpcSettings.UrlPortDaemon,
                "--password \"" + Passphrase + "\""
            };

            if (ProcessSettings.UseTestnet) {
                ProcessArguments.Add("--testnet");
            }

            if (IsAccountKeysFileExistent) {
                // Load existing account
                ProcessArguments.Add("--wallet-file \"" + ProcessSettings.FileAccountData + "\"");

                if (RpcSettings.UrlHostAccountManager != MoneroAPI.Utilities.DefaultRpcUrlHost) {
                    ProcessArguments.Add("--rpc-bind-ip " + RpcSettings.UrlHostAccountManager);
                }
                ProcessArguments.Add("--rpc-bind-port " + RpcSettings.UrlPortAccountManager);

            } else {
                // Create new account
                var directoryAccountData = ProcessSettings.DirectoryAccountData;

                if (!Directory.Exists(directoryAccountData)) Directory.CreateDirectory(directoryAccountData);
                ProcessArguments.Add("--generate-new-wallet \"" + ProcessSettings.FileAccountData + "\"");
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
                StartProcess(ProcessArguments);
            } else {
                IsWaitingForStart = true;
                DaemonProcess.Initialized += OnDaemonInitialized;
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
            }
        }

        private void OnDaemonInitialized(object sender, EventArgs e)
        {
            if (IsWaitingForStart) {
                IsWaitingForStart = false;
                StartProcess(ProcessArguments);
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
                path = ProcessSettings.DirectoryAccountBackups + DateTime.Now.ToString("yyyy-MM-dd", Utilities.InvariantCulture);
            }

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var accountName = Path.GetFileNameWithoutExtension(ProcessSettings.FileAccountData);

            var filesToBackup = Directory.GetFiles(ProcessSettings.DirectoryAccountData, accountName + "*", SearchOption.TopDirectoryOnly);
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

        private void OnProcessExited(object sender, ProcessExitedEventArgs e)
        {
            switch (e.ExitCode) {
                case 1:
                    // Invalid passphrase
                    RequestPassphrase(false);
                    break;
            }
        }
    }
}
