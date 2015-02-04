using Jojatekok.MoneroAPI.RpcManagers;
using Jojatekok.MoneroAPI.RpcManagers.AccountManager.Json.Requests;
using Jojatekok.MoneroAPI.RpcManagers.AccountManager.Json.Responses;
using Jojatekok.MoneroAPI.Settings;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Jojatekok.MoneroAPI.ProcessManagers
{
    public class AccountManager : BaseRpcProcessManager, IAccountManager
    {
        public event EventHandler<AddressReceivedEventArgs> AddressReceived;
        public event EventHandler<TransactionReceivedEventArgs> TransactionReceived;
        public event EventHandler<BalanceChangingEventArgs> BalanceChanging;

        private string _address;
        private Balance _balance;
        private readonly IList<Transaction> _transactions = new List<Transaction>();

        private bool IsTransactionReceivedEventEnabled { get; set; }

        private Timer TimerRefresh { get; set; }

        private RpcWebClient RpcWebClient { get; set; }
        private ITimerSettings TimerSettings { get; set; }
        private DaemonManager Daemon { get; set; }

        public string Address {
            get { return _address; }

            private set {
                _address = value;
                if (AddressReceived != null) AddressReceived(this, new AddressReceivedEventArgs(value));
            }
        }

        public Balance Balance {
            get { return _balance; }

            private set {
                if (BalanceChanging != null) BalanceChanging(this, new BalanceChangingEventArgs(value, Balance));
                _balance = value;
            }
        }

        public IList<Transaction> Transactions {
            get { return _transactions; }
        }

        internal AccountManager(RpcWebClient rpcWebClient, DaemonManager daemon) : base(rpcWebClient, false)
        {
            RpcAvailabilityChanged += Process_RpcAvailabilityChanged;

            RpcWebClient = rpcWebClient;
            TimerSettings = rpcWebClient.TimerSettings;
            Daemon = daemon;

            TimerRefresh = new Timer(delegate { RequestRefresh(); }, null, Timeout.Infinite, Timeout.Infinite);
        }

        private void QueryAddress()
        {
            Address = JsonPostData<AddressValueContainer>(new QueryAddress()).Result.Value;
        }

        public string QueryKey(QueryKeyParameters.KeyType keyType)
        {
            var key = JsonPostData<KeyValueContainer>(new QueryKey(keyType)).Result;
            return key != null ? key.Value : null;
        }

        private void QueryBalance()
        {
            var balance = JsonPostData<Balance>(new QueryBalance()).Result;
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
                    var transaction = transactions.Value[i];
                    transaction.Number = (uint)(i + 1);
                    // TODO: Add support for detecting transaction type

                    Transactions[i] = transaction;
                }

                // Add new transactions
                for (var i = currentTransactionCount; i < transactions.Value.Count; i++) {
                    var transaction = transactions.Value[i];
                    transaction.Number = (uint)(Transactions.Count + 1);
                    // TODO: Add support for detecting transaction type

                    Transactions.Add(transaction);
                    if (IsTransactionReceivedEventEnabled && TransactionReceived != null) {
                        TransactionReceived(this, new TransactionReceivedEventArgs(transaction));
                    }
                }
            }

            IsTransactionReceivedEventEnabled = true;
        }

        private void RequestRefresh()
        {
            TimerRefresh.Stop();
            QueryBalance();
            QueryIncomingTransfers();
            TimerRefresh.StartOnce(TimerSettings.AccountRefreshPeriod);
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

        private void Process_RpcAvailabilityChanged(object sender, EventArgs e)
        {
            if (IsRpcAvailable) {
                QueryAddress();
                RequestRefresh();

            } else {
                TimerRefresh.Stop();
            }
        }

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing) {
                TimerRefresh.Dispose();
                TimerRefresh = null;

                base.Dispose();
            }
        }
    }
}
