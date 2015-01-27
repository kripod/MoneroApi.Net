using Jojatekok.MoneroAPI.RpcManagers.AccountManager.Json.Requests;
using Jojatekok.MoneroAPI.RpcManagers.AccountManager.Json.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jojatekok.MoneroAPI.ProcessManagers
{
    public interface IAccountManager : IDisposable
    {
        event EventHandler<PassphraseRequestedEventArgs> PassphraseRequested;

        event EventHandler<AddressReceivedEventArgs> AddressReceived;
        event EventHandler<TransactionReceivedEventArgs> TransactionReceived;
        event EventHandler<BalanceChangingEventArgs> BalanceChanging;

        string Passphrase { get; set; }
        string Address { get; }
        Balance Balance { get; }

        ConcurrentReadOnlyObservableCollection<Transaction> Transactions { get; }

        void Start();
        void Stop();
        void Restart();

        string QueryKey(QueryKeyParameters.KeyType keyType);

        bool SendTransaction(IList<TransferRecipient> recipients, string paymentId, ulong mixCount);

        Task<string> BackupAsync(string path);

        Task<string> BackupAsync();
    }
}
