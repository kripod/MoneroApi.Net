using System;
using System.Threading;
using Jojatekok.MoneroAPI;
using Jojatekok.MoneroAPI.RpcManagers.AccountManager.Json.Responses;
using Jojatekok.MoneroAPI.Settings;

namespace MoneroApi.Net.Demo
{
    class Program
    {
        static MoneroClient MoneroClient { get; set; }

        static string AccountAddress { get; set; }
        static Balance AccountBalance { get; set; }

        static bool IsTransactionSendingEnabled { get; set; }

        static void Main()
        {
            // Assign a new instance of the client to a variable
            MoneroClient = new MoneroClient(
                new PathSettings(),
                new RpcSettings {
                    UrlHostDaemon = "http://xmr1.coolmining.club",
                    UrlPortDaemon = 5012
                }
            );

            // First, declare event handlers for the daemon, and then start it
            var daemon = MoneroClient.Daemon;
            daemon.NetworkInformationChanging += Daemon_NetworkInformationChanging;
            daemon.BlockchainSynced += Daemon_BlockchainSynced;
            daemon.Start();

            // Optionally, declare event handlers for the account manager, and then start it if necessary
            var accountManager = MoneroClient.AccountManager;
            accountManager.PassphraseRequested += AccountManager_PassphraseRequested;
            accountManager.AddressReceived += AccountManager_AddressReceived;
            accountManager.TransactionReceived += AccountManager_TransactionReceived;
            accountManager.BalanceChanging += AccountManager_BalanceChanging;
            accountManager.Start();

            // Wait infinitely in order to keep the application running
            while (true) {
                using (var manualResetEvent = new ManualResetEvent(false)) {
                    manualResetEvent.WaitOne();
                }
            }
        }

        static void Daemon_NetworkInformationChanging(object sender, NetworkInformationChangingEventArgs e)
        {
            // Get important data about the network's changes here
            var networkInformation = e.NewValue;
            Console.WriteLine(
                "Current block height: {0} (Downloaded: {1})",
                networkInformation.BlockHeightTotal,
                networkInformation.BlockHeightDownloaded
            );
        }

        static void Daemon_BlockchainSynced(object sender, EventArgs e)
        {
            // This event has to fire in order to allow sending transactions with the AccountManager correctly
            IsTransactionSendingEnabled = true;
        }

        static void AccountManager_PassphraseRequested(object sender, PassphraseRequestedEventArgs e)
        {
            if (e.IsFirstTime) {
                // The account to be used does not exists, create it by specifying a passphrase below
                MoneroClient.AccountManager.Passphrase = "<New account's passphrase>";

            } else {
                // The account could not have been accessed with the current passphrase given, retry with an other string
                MoneroClient.AccountManager.Passphrase = "<Insert correct passphrase here>";
            }
        }

        static void AccountManager_AddressReceived(object sender, AddressReceivedEventArgs e)
        {
            // This event fires when the opened account's public address has been loaded
            AccountAddress = e.Address;
        }

        static void AccountManager_TransactionReceived(object sender, TransactionReceivedEventArgs e)
        {
            // Whether a new transaction is sent from or received by your account, its details can be viewed here
            // You should use MoneroClient.AccountManager.Transactions instead of events whether you are binding to a DataSource property

            var transaction = e.Transaction;
            Console.WriteLine(
                "New transaction: {0} ({1} atomic units, {2})",
                transaction.TransactionId,
                transaction.Amount,
                transaction.IsAmountSpendable ? "spendable" : "not spendable"
            );
        }

        static void AccountManager_BalanceChanging(object sender, BalanceChangingEventArgs e)
        {
            // This event fires during account initialization, and on each incoming/outgoing transaction
            AccountBalance = e.NewValue;
        }
    }
}
