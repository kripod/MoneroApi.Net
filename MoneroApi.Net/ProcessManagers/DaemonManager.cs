using Jojatekok.MoneroAPI.RpcManagers;
using Jojatekok.MoneroAPI.RpcManagers.Daemon.Http.Responses;
using Jojatekok.MoneroAPI.RpcManagers.Daemon.Json.Requests;
using Jojatekok.MoneroAPI.RpcManagers.Daemon.Json.Responses;
using Jojatekok.MoneroAPI.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Jojatekok.MoneroAPI.ProcessManagers
{
    public class DaemonManager : BaseRpcProcessManager
    {
        public event EventHandler BlockchainSynced;
        public event EventHandler<NetworkInformationChangingEventArgs> NetworkInformationChanging;

        private bool _isBlockchainSynced;
        private NetworkInformation _networkInformation;

        private static readonly string[] ProcessArgumentsDefault = { "--log-level 0" };
        private List<string> ProcessArgumentsExtra { get; set; }

        private Timer TimerQueryNetworkInformation { get; set; }

        private RpcWebClient RpcWebClient { get; set; }
        private ITimerSettings TimerSettings { get; set; }

        public bool IsBlockchainSynced {
            get { return _isBlockchainSynced; }

            private set {
                _isBlockchainSynced = value;
                if (BlockchainSynced != null && value) BlockchainSynced(this, EventArgs.Empty);
            }
        }

        public NetworkInformation NetworkInformation {
            get { return _networkInformation; }

            private set {
                if (value == NetworkInformation) return;

                if (NetworkInformationChanging != null) {
                    NetworkInformationChanging(this, new NetworkInformationChangingEventArgs(value, NetworkInformation));
                }
                _networkInformation = value;
            }
        }

        internal DaemonManager(RpcWebClient rpcWebClient) : base(rpcWebClient, true)
        {
            RpcAvailabilityChanged += Process_RpcAvailabilityChanged;

            TimerQueryNetworkInformation = new Timer(delegate { QueryNetworkInformation(); });

            RpcWebClient = rpcWebClient;
            TimerSettings = rpcWebClient.TimerSettings;

            var rpcSettings = RpcWebClient.RpcSettings;
            var pathSettings = RpcWebClient.PathSettings;

            if (!pathSettings.StartDaemonProcess) {
                IsRpcAvailable = true;
                return;
            }

            ProcessArgumentsExtra = new List<string>(3) {
                //"--data-dir \"" + paths.DirectoryDaemonData + "\"",
                "--rpc-bind-port " + rpcSettings.UrlPortDaemon
            };

            if (rpcSettings.UrlHostDaemon != Utilities.DefaultRpcUrlHostDaemon) {
                ProcessArgumentsExtra.Add("--rpc-bind-ip " + rpcSettings.UrlHostDaemon);
            }

            // TODO: Remove this temporary fix
            ProcessArgumentsExtra.Add("--data-dir \"" + pathSettings.DirectoryDaemonData);
        }

        public void Start()
        {
            if (RpcWebClient.PathSettings.StartDaemonProcess) {
                StartProcess(ProcessArgumentsDefault.Concat(ProcessArgumentsExtra).ToArray());
            }
        }

        public void Stop()
        {
            if (RpcWebClient.PathSettings.StartDaemonProcess) {
                KillBaseProcess();
            }
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        private void QueryNetworkInformation()
        {
            TimerQueryNetworkInformation.Stop();

            var output = HttpPostData<NetworkInformation>(HttpRpcCommands.DaemonQueryNetworkInformation);
            if (output != null && output.BlockHeightTotal != 0) {
                var blockHeaderLast = QueryBlockHeaderLast();
                if (blockHeaderLast != null) {
                    output.BlockTimeCurrent = blockHeaderLast.Timestamp;

                    NetworkInformation = output;

                    if (output.BlockHeightRemaining == 0 && !IsBlockchainSynced) {
                        IsBlockchainSynced = true;
                    }
                }
            }

            TimerQueryNetworkInformation.StartOnce(TimerSettings.DaemonQueryNetworkInformationPeriod);
        }

        public ulong QueryCurrentBlockHeight()
        {
            var output = HttpPostData<BlockHeightContainer>(HttpRpcCommands.DaemonQueryCurrentBlockHeight);
            if (output != null) {
                return output.BlockHeight;
            }

            return 0;
        }

        public BlockHeader QueryBlockHeaderLast()
        {
            var output = JsonPostData<BlockHeaderValueContainer>(new QueryBlockHeaderLast()).Result;
            if (output != null) {
                return output.Value;
            }

            return null;
        }

        public BlockHeader QueryBlockHeaderByHeight(ulong height)
        {
            var output = JsonPostData<BlockHeaderValueContainer>(new QueryBlockHeaderByHeight(height)).Result;
            if (output != null) {
                return output.Value;
            }

            return null;
        }

        public BlockHeader QueryBlockHeaderByHash(string hash)
        {
            var output = JsonPostData<BlockHeaderValueContainer>(new QueryBlockHeaderByHash(hash)).Result;
            if (output != null) {
                return output.Value;
            }

            return null;
        }

        public MiningStatus QueryMiningStatus()
        {
            var output = HttpPostData<MiningStatus>(HttpRpcCommands.DaemonQueryMiningStatus);
            if (output != null) {
                return output;
            }

            return null;
        }

        private void Process_RpcAvailabilityChanged(object sender, EventArgs e)
        {
            if (IsRpcAvailable) {
                TimerQueryNetworkInformation.StartImmediately(TimerSettings.DaemonQueryNetworkInformationPeriod);

            } else {
                TimerQueryNetworkInformation.Stop();
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
                TimerQueryNetworkInformation.Dispose();
                TimerQueryNetworkInformation = null;

                if (RpcWebClient.PathSettings.StartDaemonProcess) {
                    // Safe shutdown
                    SendConsoleCommand("exit");
                }

                IsDisposeProcessKillNecessary = false;
                base.Dispose();
            }
        }
    }
}
