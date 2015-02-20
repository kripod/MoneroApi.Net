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

        public event EventHandler<AccountAddressReceivedEventArgs> AddressReceived;
        public event EventHandler<TransactionReceivedEventArgs> TransactionReceived;
        public event EventHandler<TransactionChangedEventArgs> TransactionChanged;
        public event EventHandler<AccountBalanceChangedEventArgs> BalanceChanged;

        private bool _isInitialized;
        private bool _isTransactionListInitialized;
        private string _address;
        private AccountBalance _balance;
        private IList<Transaction> _transactions = new List<Transaction>();
        private IList<TransactionOutput> _transactionOutputs = new List<TransactionOutput>();

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

        private ITimerSettings TimerSettings { get; set; }

        public string Address {
            get { return _address; }

            private set {
                _address = value;
                if (AddressReceived != null) AddressReceived(this, new AccountAddressReceivedEventArgs(value));
            }
        }

        public AccountBalance Balance {
            get { return _balance; }

            private set {
                if (value == Balance) return;

                _balance = value;
                if (BalanceChanged != null) BalanceChanged(this, new AccountBalanceChangedEventArgs(value));
            }
        }

        public IList<Transaction> Transactions {
            get { return _transactions; }
            private set { _transactions = value; }
        }

        public IList<TransactionOutput> TransactionOutputs {
            get { return _transactionOutputs; }
            private set { _transactionOutputs = value; }
        }

        internal AccountRpcManager(RpcWebClient rpcWebClient) : base(rpcWebClient, false)
        {
            TimerSettings = rpcWebClient.TimerSettings;

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
            var transactionOutputsContainer = JsonPostData<TransactionOutputListValueContainer>(new QueryIncomingTransfers()).Result;
            if (transactionOutputsContainer == null) return;

            var transactionOutputs = transactionOutputsContainer.Value;
            var transactionOutputsCount = transactionOutputs.Count;
            
            TransactionOutputs = transactionOutputs;

            if (transactionOutputsCount != 0) {
                var transactions = new List<Transaction>();

                var previousTransactionId = transactionOutputs[0].TransactionId;
                var transactionIndex = 0U;
                var transaction = new Transaction {
                    TransactionId = previousTransactionId,
                    Index = transactionIndex
                };

                for (var i = 0; i < transactionOutputsCount; i++) {
                    var transactionOutput = transactionOutputs[i];
                    var transactionId = transactionOutput.TransactionId;

                    if (transactionId != previousTransactionId) {
                        transaction = new Transaction {
                            TransactionId = transactionId,
                            Index = transactionIndex
                        };

                        transactions.Add(transaction);
                        previousTransactionId = transactionId;
                        transactionIndex += 1;
                    }

                    if (transactionOutput.IsAmountSpendable) {
                        transaction.AmountSpendable += transactionOutput.Amount;
                    } else {
                        transaction.AmountUnspendable += transactionOutput.Amount;
                    }
                }

                if (IsTransactionListInitialized) {
                    var previousTransactionsCount = Transactions.Count;

                    if (TransactionChanged != null) {
                        // Notify about changed transactions
                        for (var i = 0; i < previousTransactionsCount; i++) {
                            var oldTransaction = Transactions[i];
                            var newTransaction = transactions[i];

                            if (newTransaction.AmountSpendable != oldTransaction.AmountSpendable) {
                                TransactionChanged(this, new TransactionChangedEventArgs(i, newTransaction));
                            }
                        }
                    }

                    if (TransactionReceived != null) {
                        // Notify about new transactions
                        for (var i = previousTransactionsCount; i < transactions.Count; i++) {
                            TransactionReceived(this, new TransactionReceivedEventArgs(transactions[i]));
                        }
                    }
                }

                Transactions = transactions;
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
                if (IsInitialized) {
                    RequestSaveAccount();
                }

                TimerRefresh.Dispose();
                TimerRefresh = null;
            }
        }
    }
}
