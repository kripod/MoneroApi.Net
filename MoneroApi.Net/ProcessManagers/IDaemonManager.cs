using Jojatekok.MoneroAPI.RpcManagers.Daemon.Http.Responses;
using System;

namespace Jojatekok.MoneroAPI.ProcessManagers
{
    public interface IDaemonManager : IDisposable
    {
        event EventHandler BlockchainSynced;
        event EventHandler<NetworkInformationChangingEventArgs> NetworkInformationChanging;

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
