using System;
using System.Collections.Generic;

namespace Jojatekok.MoneroAPI.RpcManagers
{
    public interface IAccountRpcManager : IDisposable
    {
        event EventHandler Initialized;

        event EventHandler<AccountAddressReceivedEventArgs> AddressReceived;
        event EventHandler<TransactionReceivedEventArgs> TransactionReceived;
        event EventHandler<TransactionChangedEventArgs> TransactionChanged;
        event EventHandler<AccountBalanceChangedEventArgs> BalanceChanged;

        string Address { get; }
        AccountBalance Balance { get; }
        IList<Transaction> Transactions { get; }

        void Initialize();

        string QueryKey(AccountKeyType keyType);

        IList<Payment> QueryPayments(IList<string> paymentIds, ulong minimumBlockHeight);
        IList<Payment> QueryPayments(IList<string> paymentIds);
        IList<Payment> QueryPayments(ulong minimumBlockHeight);
        IList<Payment> QueryPayments();

        bool SendTransaction(IList<TransferRecipient> recipients, string paymentId, ulong mixCount);
        bool SendTransaction(IList<TransferRecipient> recipients, string paymentId);
        bool SendTransaction(IList<TransferRecipient> recipients);
        bool SendTransaction(TransferRecipient recipient, string paymentId, ulong mixCount);
        bool SendTransaction(TransferRecipient recipient, string paymentId);
        bool SendTransaction(TransferRecipient recipient);

        void DisposeSafely();
    }
}
