using Jojatekok.MoneroAPI.RpcUtilities;
using Jojatekok.MoneroAPI.RpcUtilities.Daemon.Http.Responses;
using Jojatekok.MoneroAPI.RpcUtilities.Daemon.Json.Requests;
using Jojatekok.MoneroAPI.RpcUtilities.Daemon.Json.Responses;
using Jojatekok.MoneroAPI.Settings;
using System;
using System.Threading;

namespace Jojatekok.MoneroAPI.RpcManagers
{
    public class DaemonRpcManager : BaseRpcManager, IDaemonRpcManager
    {
        public event EventHandler BlockchainSynced;
        public event EventHandler<NetworkInformationChangingEventArgs> NetworkInformationChanging;

        private bool _isBlockchainSynced;
        private NetworkInformation _networkInformation;

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

        internal DaemonRpcManager(RpcWebClient rpcWebClient) : base(rpcWebClient, true)
        {
            RpcWebClient = rpcWebClient;
            TimerSettings = rpcWebClient.TimerSettings;

            TimerQueryNetworkInformation = new Timer(
                delegate { QueryNetworkInformation(); },
                null,
                Timeout.Infinite,
                Timeout.Infinite
            );
        }

        public void Initialize()
        {
            TimerQueryNetworkInformation.StartImmediately(TimerSettings.DaemonQueryNetworkInformationPeriod);
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
            var output = HttpPostData<BlockHeightValueContainer>(HttpRpcCommands.DaemonQueryCurrentBlockHeight);
            if (output != null) {
                return output.Value;
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing) {
                TimerQueryNetworkInformation.Dispose();
                TimerQueryNetworkInformation = null;
            }
        }
    }
}
