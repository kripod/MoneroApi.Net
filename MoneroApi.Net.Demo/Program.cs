using Jojatekok.MoneroAPI.RpcUtilities.AccountManager.Json.Responses;
using System;
using System.Threading;

// Include the namespace below in order to access process manager functionality
using Jojatekok.MoneroAPI.Extensions;

namespace Jojatekok.MoneroAPI.Demo
{
    class Program
    {
        static MoneroRpcManager MoneroRpcManager { get; set; }

        static string AccountAddress { get; set; }
        static Balance AccountBalance { get; set; }

        static bool IsTransactionSendingEnabled { get; set; }

        static void Main()
        {
            // Assign a new instance of the client to a variable
            MoneroRpcManager = new MoneroRpcManager(
                Config.ClientRpcSettings,
                Config.ClientTimerSettings
            );

            // First, declare event handlers for the daemon, and then initialize it
            var daemon = MoneroRpcManager.Daemon;
            daemon.NetworkInformationChanging += Daemon_NetworkInformationChanging;
            daemon.BlockchainSynced += Daemon_BlockchainSynced;

            // Daemon functions will not be available if the line below is commented out
            daemon.Initialize();

            // Optionally, declare event handlers for the account manager, and then initialize it if necessary
            var accountManager = MoneroRpcManager.AccountManager;
            accountManager.AddressReceived += AccountManager_AddressReceived;
            accountManager.TransactionReceived += AccountManager_TransactionReceived;
            accountManager.BalanceChanging += AccountManager_BalanceChanging;

            // The account manager's functions will not be available if the line below is commented out
            //accountManager.Initialize();

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

        static void AccountManager_AddressReceived(object sender, AddressReceivedEventArgs e)
        {
            // This event fires when the opened account's public address has been loaded
            AccountAddress = e.Address;
        }

        static void AccountManager_TransactionReceived(object sender, TransactionReceivedEventArgs e)
        {
            // Whether a new transaction is sent from or received by your account, its details can be viewed here
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
