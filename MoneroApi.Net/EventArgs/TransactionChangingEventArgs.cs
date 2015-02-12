namespace Jojatekok.MoneroAPI
{
    public class TransactionChangingEventArgs : ValueChangingEventArgs<Transaction>
    {
        public Transaction NewValue { get; private set; }
        public Transaction OldValue { get; private set; }

        public int TransactionIndex { get; private set; }

        internal TransactionChangingEventArgs(Transaction newValue, Transaction oldValue, int transactionIndex) : base(newValue, oldValue)
        {
            NewValue = newValue;
            OldValue = oldValue;
            TransactionIndex = transactionIndex;
        }
    }
}
