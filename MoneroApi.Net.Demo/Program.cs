using System;
using System.Threading;

// Include the namespace below in order to access process manager functionality
using Jojatekok.MoneroAPI.Extensions;

namespace Jojatekok.MoneroAPI.Demo
{
    class Program
    {
        static MoneroProcessManager MoneroProcessManager { get; set; }
        static MoneroRpcManager MoneroRpcManager { get; set; }

        static string AccountAddress { get; set; }
        static AccountBalance AccountBalance { get; set; }

        static bool IsTransactionSendingEnabled { get; set; }

        static void Main()
        {
            StartDemo();

            // Wait infinitely in order to keep the application running
            while (true) {
                using (var manualResetEvent = new ManualResetEvent(false)) {
                    manualResetEvent.WaitOne();
                }
            }
        }

        static void StartDemo()
        {
            // Assign a new instance of the process manager to a variable
            MoneroProcessManager = new MoneroProcessManager(
                Config.ClientRpcSettings,
                Config.ClientAccountManagerPathSettings,
                Config.ClientDaemonPathSettings
            );

            // Assign a new instance of the RPC manager to a variable
            MoneroRpcManager = new MoneroRpcManager(
                Config.ClientRpcSettings,
                Config.ClientTimerSettings
            );

            // First, declare event handlers for the daemon
            var daemonRpc = MoneroRpcManager.Daemon;
            daemonRpc.NetworkInformationChanging += Daemon_NetworkInformationChanging;
            daemonRpc.BlockchainSynced += Daemon_BlockchainSynced;

            // Optionally, declare event handlers for the account manager
            var accountManagerRpc = MoneroRpcManager.AccountManager;
            accountManagerRpc.AddressReceived += AccountManager_AddressReceived;
            accountManagerRpc.TransactionReceived += AccountManager_TransactionReceived;
            accountManagerRpc.BalanceChanging += AccountManager_BalanceChanging;

            if (Config.IsDaemonProcessRemote) {
                // Daemon RPC functions will not be available if the line below is commented out
                daemonRpc.Initialize();

            } else {
                // Initialize the daemon RPC manager as soon as the corresponding process is available
                var daemonProcess = MoneroProcessManager.Daemon;
                daemonProcess.Initialized += delegate {
                    // Daemon RPC functions will not be available if the line below is commented out
                    daemonRpc.Initialize();
                };
                daemonProcess.Start();
            }

            if (Config.IsAccountManagerProcessRemote) {
                // The account manager's RPC functions will not be available if the line below is commented out
                accountManagerRpc.Initialize();

            } else {
                // Initialize the account manager's RPC wrapper as soon as the corresponding process is available
                var accountManagerProcess = MoneroProcessManager.AccountManager;
                accountManagerProcess.Initialized += delegate {
                    // The account manager's RPC functions will not be available if the line below is commented out
                    accountManagerRpc.Initialize();
                };
                accountManagerProcess.PassphraseRequested += delegate {
                    accountManagerProcess.Passphrase = "x";
                };
                accountManagerProcess.Start();
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

        static void AccountManager_BalanceChanging(object sender, AccountBalanceChangingEventArgs e)
        {
            // This event fires during account initialization, and on each incoming/outgoing transaction
            AccountBalance = e.NewValue;
        }
    }
}
