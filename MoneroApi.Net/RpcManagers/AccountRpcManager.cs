using Jojatekok.MoneroAPI.RpcUtilities;
using Jojatekok.MoneroAPI.RpcUtilities.AccountManager.Json.Requests;
using Jojatekok.MoneroAPI.RpcUtilities.AccountManager.Json.Responses;
using Jojatekok.MoneroAPI.Settings;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Jojatekok.MoneroAPI.RpcManagers
{
    public class AccountRpcManager : BaseRpcManager, IAccountRpcManager
    {
        public event EventHandler Initialized;

        public event EventHandler<AddressReceivedEventArgs> AddressReceived;
        public event EventHandler<TransactionReceivedEventArgs> TransactionReceived;
        public event EventHandler<TransactionChangingEventArgs> TransactionChanging;
        public event EventHandler<AccountBalanceChangingEventArgs> BalanceChanging;

        private bool _isInitialized;
        private bool _isTransactionListInitialized;
        private string _address;
        private AccountBalance _balance;
        private readonly IList<Transaction> _transactions = new List<Transaction>();

        private bool IsInitialized {
            get { return _isInitialized; }

            set {
                if (value == _isInitialized) return;
                _isInitialized = value;

                if (value && Initialized != null) {
                    Initialized(this, EventArgs.Empty);
                }
            }
        }

        private bool IsTransactionListInitialized {
            get { return _isTransactionListInitialized; }

            set {
                if (value == _isTransactionListInitialized) return;
                _isTransactionListInitialized = value;

                if (value && Address != null) {
                    IsInitialized = true;
                }
            }
        }

        private Timer TimerRefresh { get; set; }

        private RpcWebClient RpcWebClient { get; set; }
        private ITimerSettings TimerSettings { get; set; }
        private IDaemonRpcManager Daemon { get; set; }

        public string Address {
            get { return _address; }

            private set {
                _address = value;
                if (AddressReceived != null) AddressReceived(this, new AddressReceivedEventArgs(value));
            }
        }

        public AccountBalance Balance {
            get { return _balance; }

            private set {
                if (BalanceChanging != null) BalanceChanging(this, new AccountBalanceChangingEventArgs(value, Balance));
                _balance = value;
            }
        }

        public IList<Transaction> Transactions {
            get { return _transactions; }
        }

        internal AccountRpcManager(RpcWebClient rpcWebClient, IDaemonRpcManager daemon) : base(rpcWebClient, false)
        {
            RpcWebClient = rpcWebClient;
            TimerSettings = rpcWebClient.TimerSettings;
            Daemon = daemon;

            TimerRefresh = new Timer(
                delegate { RequestRefresh(); },
                null,
                Timeout.Infinite,
                Timeout.Infinite
            );
        }

        public void Initialize()
        {
            TimerRefresh.StartImmediately(TimerSettings.AccountRefreshPeriod);
            QueryAddress();
        }

        private void QueryAddress()
        {
            Address = JsonPostData<AddressValueContainer>(new QueryAddress()).Result.Value;
            if (IsTransactionListInitialized) {
                IsInitialized = true;
            }
        }

        public string QueryKey(AccountKeyType keyType)
        {
            var key = JsonPostData<KeyValueContainer>(new QueryKey(keyType)).Result;
            return key != null ? key.Value : null;
        }

        private void QueryBalance()
        {
            var balance = JsonPostData<AccountBalance>(new QueryBalance()).Result;
            if (balance != null) {
                Balance = balance;
            }
        }

        private void QueryIncomingTransfers()
        {
            var transactions = JsonPostData<TransactionListValueContainer>(new QueryIncomingTransfers()).Result;

            if (transactions != null) {
                var currentTransactionCount = Transactions.Count;

                // Update existing transactions
                for (var i = currentTransactionCount - 1; i >= 0; i--) {
                    var newTransaction = transactions.Value[i];
                    newTransaction.Number = (uint)(i + 1);
                    // TODO: Add support for detecting transaction type

                    var oldTransaction = Transactions[i];
                    if (newTransaction.IsAmountSpendable != oldTransaction.IsAmountSpendable) {
                        if (IsTransactionListInitialized && TransactionChanging != null) {
                            TransactionChanging(this, new TransactionChangingEventArgs(newTransaction, oldTransaction, i));
                        }
                        Transactions[i] = newTransaction;
                    }
                }

                // Add new transactions
                for (var i = currentTransactionCount; i < transactions.Value.Count; i++) {
                    var newTransaction = transactions.Value[i];
                    newTransaction.Number = (uint)(Transactions.Count + 1);
                    // TODO: Add support for detecting transaction type

                    Transactions.Add(newTransaction);
                    if (IsTransactionListInitialized && TransactionReceived != null) {
                        TransactionReceived(this, new TransactionReceivedEventArgs(newTransaction));
                    }
                }
            }

            IsTransactionListInitialized = true;
        }

        private void RequestRefresh()
        {
            TimerRefresh.Stop();
            QueryBalance();
            QueryIncomingTransfers();
            TimerRefresh.StartOnce(TimerSettings.AccountRefreshPeriod);
        }

        private void RequestSaveAccount()
        {
            JsonPostData(new RequestSaveAccount());
        }

        public bool SendTransaction(IList<TransferRecipient> recipients, string paymentId, ulong mixCount)
        {
            if (recipients == null || recipients.Count == 0) return false;

            var parameters = new SendTransferSplitParameters(recipients) {
                PaymentId = paymentId,
                MixCount = mixCount
            };

            var output = JsonPostData<TransactionIdListValueContainer>(new SendTransferSplit(parameters));
            if (output == null) return false;

            ulong amountTotal = 0;
            for (var i = recipients.Count - 1; i >= 0; i--) {
                amountTotal += recipients[i].Amount;
            }
            
            if (TransactionReceived != null) {
                TransactionReceived(this, new TransactionReceivedEventArgs(new Transaction {
                    Type = TransactionType.Send,
                    Amount = amountTotal
                }));
            }

            RequestRefresh();
            return true;
        }

        public bool SendTransaction(IList<TransferRecipient> recipients, string paymentId)
        {
            return SendTransaction(recipients, paymentId, Utilities.DefaultTransactionMixCount);
        }

        public bool SendTransaction(IList<TransferRecipient> recipients)
        {
            return SendTransaction(recipients, null, Utilities.DefaultTransactionMixCount);
        }

        public bool SendTransaction(TransferRecipient recipient, string paymentId, ulong mixCount)
        {
            return SendTransaction(new List<TransferRecipient> { recipient }, paymentId, mixCount);
        }

        public bool SendTransaction(TransferRecipient recipient, string paymentId)
        {
            return SendTransaction(new List<TransferRecipient> { recipient }, paymentId, Utilities.DefaultTransactionMixCount);
        }

        public bool SendTransaction(TransferRecipient recipient)
        {
            return SendTransaction(new List<TransferRecipient> { recipient }, null, Utilities.DefaultTransactionMixCount);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing) {
                RequestSaveAccount();

                TimerRefresh.Dispose();
                TimerRefresh = null;
            }
        }
    }
}
