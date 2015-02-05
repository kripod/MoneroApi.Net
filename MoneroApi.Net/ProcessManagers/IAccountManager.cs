using Jojatekok.MoneroAPI.RpcManagers.AccountManager.Json.Requests;
using Jojatekok.MoneroAPI.RpcManagers.AccountManager.Json.Responses;
using System;
using System.Collections.Generic;

namespace Jojatekok.MoneroAPI.ProcessManagers
{
    public interface IAccountManager : IDisposable
    {
        event EventHandler<AddressReceivedEventArgs> AddressReceived;
        event EventHandler<TransactionReceivedEventArgs> TransactionReceived;
        event EventHandler<BalanceChangingEventArgs> BalanceChanging;

        string Address { get; }
        Balance Balance { get; }
        IList<Transaction> Transactions { get; }

        void Initialize();

        string QueryKey(QueryKeyParameters.KeyType keyType);

        bool SendTransaction(IList<TransferRecipient> recipients, string paymentId, ulong mixCount);
    }
}
