using Jojatekok.MoneroAPI.RpcUtilities.Daemon.Http.Responses;
using System;

namespace Jojatekok.MoneroAPI.RpcManagers
{
    public interface IDaemonRpcManager : IDisposable
    {
        event EventHandler BlockchainSynced;
        event EventHandler<NetworkInformationChangedEventArgs> NetworkInformationChanged;

        bool IsBlockchainSynced { get; }

        NetworkInformation NetworkInformation { get; }

        void Initialize();

        ulong QueryCurrentBlockHeight();

        BlockHeader QueryBlockHeaderLast();
        BlockHeader QueryBlockHeaderByHeight(ulong height);
        BlockHeader QueryBlockHeaderByHash(string hash);

        MiningStatus QueryMiningStatus();
    }
}
