using System;

// Include the namespace below in order to access process manager functionality
using Jojatekok.MoneroAPI.Extensions;

namespace Jojatekok.MoneroAPI.Demo
{
    static class Program
    {
        static MoneroProcessManager MoneroProcessManager { get; set; }
        static MoneroRpcManager MoneroRpcManager { get; set; }

        static string AccountAddress { get; set; }
        static AccountBalance AccountBalance { get; set; }

        static bool IsTransactionSendingEnabled { get; set; }

        static void Main()
        {
            Console.WriteLine("Press any key to exit safely!");
            StartDemo();

            Console.ReadKey(true);
            if (MoneroRpcManager != null) MoneroRpcManager.Dispose();
            if (MoneroProcessManager != null) MoneroProcessManager.Dispose();
        }

        static void StartDemo()
        {
            // Assign a new instance of the process manager to a variable
            MoneroProcessManager = new MoneroProcessManager(
                Config.ClientRpcSettings,
                Config.ClientAccountManagerProcessSettings,
                Config.ClientDaemonProcessSettings
            );

            // Assign a new instance of the RPC manager to a variable
            MoneroRpcManager = new MoneroRpcManager(
                Config.ClientRpcSettings,
                Config.ClientTimerSettings
            );

            // First, declare event handlers for the daemon
            var daemonRpc = MoneroRpcManager.Daemon;
            daemonRpc.NetworkInformationChanged += OnDaemonNetworkInformationChanged;
            daemonRpc.BlockchainSynced += OnDaemonBlockchainSynced;

            // Optionally, declare event handlers for the account manager
            var accountManagerRpc = MoneroRpcManager.AccountManager;
            accountManagerRpc.AddressReceived += OnAccountManagerAddressReceived;
            accountManagerRpc.TransactionReceived += OnAccountManagerTransactionReceived;
            accountManagerRpc.BalanceChanged += OnAccountManagerBalanceChanged;

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

        static void OnDaemonNetworkInformationChanged(object sender, NetworkInformationChangedEventArgs e)
        {
            // Get important data about the network's changes here
            var networkInformation = e.NetworkInformation;
            Console.WriteLine(
                "Current block height: {0} (Downloaded: {1})",
                networkInformation.BlockHeightTotal,
                networkInformation.BlockHeightDownloaded
            );
        }

        static void OnDaemonBlockchainSynced(object sender, EventArgs e)
        {
            // This event has to fire in order to allow sending transactions with the AccountManager correctly
            IsTransactionSendingEnabled = true;
        }

        static void OnAccountManagerAddressReceived(object sender, AccountAddressReceivedEventArgs e)
        {
            // This event fires when the opened account's public address has been loaded
            AccountAddress = e.AccountAddress;
        }

        static void OnAccountManagerTransactionReceived(object sender, TransactionReceivedEventArgs e)
        {
            // Whether a new transaction is sent from or received by your account, its details can be viewed here
            var transaction = e.Transaction;
            Console.WriteLine(
                "New transaction: {0}" + Environment.NewLine +
                "    Spendable: {1}" + Environment.NewLine +
                "    Unspendable: {2}",
                transaction.TransactionId,
                Utilities.CoinAtomicValueToString(transaction.AmountSpendable),
                Utilities.CoinAtomicValueToString(transaction.AmountUnspendable)
            );
        }

        static void OnAccountManagerBalanceChanged(object sender, AccountBalanceChangedEventArgs e)
        {
            // This event fires during account initialization, and on each incoming/outgoing transaction
            AccountBalance = e.AccountBalance;
        }
    }
}
