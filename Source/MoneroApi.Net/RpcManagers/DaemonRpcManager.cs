using Jojatekok.MoneroAPI.RpcUtilities;
using Jojatekok.MoneroAPI.RpcUtilities.Daemon.Http.Requests;
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
        public event EventHandler<NetworkInformationChangedEventArgs> NetworkInformationChanged;

        private bool _isBlockchainSynced;
        private NetworkInformation _networkInformation;

        private Timer TimerQueryNetworkInformation { get; set; }

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

                _networkInformation = value;
                if (NetworkInformationChanged != null) NetworkInformationChanged(this, new NetworkInformationChangedEventArgs(value));
            }
        }

        internal DaemonRpcManager(RpcWebClient rpcWebClient) : base(rpcWebClient, true)
        {
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
            var blockHeaderContainer = JsonPostData<BlockHeaderValueContainer>(new QueryBlockHeaderLast());
            return blockHeaderContainer != null && blockHeaderContainer.Error == null ? blockHeaderContainer.Result.Value : null;
        }

        public BlockHeader QueryBlockHeaderByHeight(ulong height)
        {
            var blockHeaderContainer = JsonPostData<BlockHeaderValueContainer>(new QueryBlockHeaderByHeight(height));
            return blockHeaderContainer != null && blockHeaderContainer.Error == null ? blockHeaderContainer.Result.Value : null;
        }

        public BlockHeader QueryBlockHeaderByHash(string hash)
        {
            var blockHeaderContainer = JsonPostData<BlockHeaderValueContainer>(new QueryBlockHeaderByHash(hash));
            return blockHeaderContainer != null && blockHeaderContainer.Error == null ? blockHeaderContainer.Result.Value : null;
        }

        public MiningStatus QueryMiningStatus()
        {
            var output = HttpPostData<MiningStatus>(HttpRpcCommands.DaemonQueryMiningStatus);
            if (output != null) {
                return output;
            }

            return null;
        }

        public bool RequestMiningStart(string accountAddress, ulong threadsCount)
        {
            return HttpPostData(HttpRpcCommands.DaemonRequestMiningStart, new RequestMiningStart(accountAddress, threadsCount));
        }

        public bool RequestMiningStop()
        {
            return HttpPostData(HttpRpcCommands.DaemonRequestMiningStop);
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
