using System;

namespace Jojatekok.MoneroAPI
{
    public class TransactionChangedEventArgs : EventArgs
    {
        public int TransactionIndex { get; private set; }
        public Transaction TransactionNewValue { get; private set; }

        internal TransactionChangedEventArgs(int transactionIndex, Transaction transactionNewValue)
        {
            TransactionIndex = transactionIndex;
            TransactionNewValue = transactionNewValue;
        }
    }
}
