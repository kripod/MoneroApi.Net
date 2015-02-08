using System;
using System.Collections.Generic;

namespace Jojatekok.MoneroAPI.RpcManagers
{
    public interface IAccountRpcManager : IDisposable
    {
        event EventHandler Initialized;

        event EventHandler<AddressReceivedEventArgs> AddressReceived;
        event EventHandler<TransactionReceivedEventArgs> TransactionReceived;
        event EventHandler<TransactionChangingEventArgs> TransactionChanging;
        event EventHandler<AccountBalanceChangingEventArgs> BalanceChanging;

        string Address { get; }
        AccountBalance Balance { get; }
        IList<Transaction> Transactions { get; }

        void Initialize();

        string QueryKey(AccountKeyType keyType);

        bool SendTransaction(IList<TransferRecipient> recipients, string paymentId, ulong mixCount);
    }
}
