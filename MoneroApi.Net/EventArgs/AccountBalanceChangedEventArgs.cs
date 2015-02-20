using System;

namespace Jojatekok.MoneroAPI
{
    public class AccountBalanceChangedEventArgs : EventArgs
    {
        public AccountBalance AccountBalance { get; private set; }

        internal AccountBalanceChangedEventArgs(AccountBalance accountBalance)
        {
            AccountBalance = accountBalance;
        }
    }
}
