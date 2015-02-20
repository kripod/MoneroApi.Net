using System;

namespace Jojatekok.MoneroAPI
{
    public class AccountAddressReceivedEventArgs : EventArgs
    {
        public string AccountAddress { get; private set; }

        internal AccountAddressReceivedEventArgs(string accountAddress)
        {
            AccountAddress = AccountAddress;
        }
    }
}
